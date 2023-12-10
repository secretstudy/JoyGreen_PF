using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Diagnostics;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Xml;

namespace JoyGreen_PF
{
    public class Set_DNS
    {
        // 설정된 DNS 값 얻기
        const int MAX_ADAPTER_NAME = 128;
        const int MAX_HOSTNAME_LEN = 128;
        const int MAX_DOMAIN_NAME_LEN = 128;
        const int MAX_SCOPE_ID_LEN = 256;
        const int MAX_ADAPTER_DESCRIPTION_LENGTH = 128;
        const int MAX_ADAPTER_NAME_LENGTH = 256;
        const int MAX_ADAPTER_ADDRESS_LENGTH = 8;
        const int DEFAULT_MINIMUM_ENTITIES = 32;

        const int ERROR_BUFFER_OVERFLOW = 111;
        const int ERROR_INSUFFICIENT_BUFFER = 122;
        const int ERROR_SUCCESS = 0;


        private static bool isDNSConfigured = false; // DNS 설정을 완료했는지 여부를 나타내는 변수


        //Iphlpapi.dll로부터 함수를 가져옵니다
        //이 DLL은 주로 IP 도우미 API를 포함하고 있습니다. 
        //CharSet.Auto는 문자 집합 설정을 자동으로 맞춥니다
        [DllImport("Iphlpapi.dll", CharSet = CharSet.Auto)]

        //Iphlpapi.dll에 있는 함수를 외부에서 가져온 것입니다.
        //네트워크 매개변수를 가져오는 역할을 합니다.
        //네트워크 매개변수는 Byte[] PFixedInfoBuffer에 저장되고, 해당 변수의 크기는 ref int size를 통해 참조됩니다.
        private static extern int GetNetworkParams(Byte[] PFixedInfoBuffer, ref int size);

        // Kernel32.dll로부터 CopyMemory 함수를 가져옵니다.
        // CopyMemory는 메모리를 복사하는 함수입니다.
        [DllImport("Kernel32.dll", EntryPoint = "CopyMemory")]

        //ByteArray_To_FixedInfo라는 함수를 선언합니다.
        // 이 함수는 Kernel32.dll에 있는 CopyMemory 함수를 FixedInfo 구조체에 바이트 배열을 복사하는 데 사용
        private static extern void ByteArray_To_FixedInfo(ref FixedInfo dst, Byte[] src, int size);

        [DllImport("Kernel32.dll", EntryPoint = "CopyMemory")]

        //IntPtr_To_IPAddrString라는 함수를 선언합니다.
        //이 함수는 Kernel32.dll에 있는 CopyMemory 함수를 사용하여
        //IPAddrString 구조체에 포인터로부터 IP 주소 문자열을 복사하는 데 사용됩니다.
        private static extern void IntPtr_To_IPAddrString(ref IPAddrString dst, IntPtr src, int size);

        [DllImport("kernel32")]

        public static extern Int32 GetLastError();





        [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Ansi)]
        struct FixedInfo
        {
            [MarshalAs(UnmanagedType.ByValTStr, SizeConst = MAX_HOSTNAME_LEN + 4)]
            public String HostName;

            [MarshalAs(UnmanagedType.ByValTStr, SizeConst = MAX_DOMAIN_NAME_LEN + 4)]
            public String DomainName;

            public IntPtr CurrentServerList;
            public IPAddrString DnsServerList;
            public NodeType_T NodeType;

            [MarshalAs(UnmanagedType.ByValTStr, SizeConst = MAX_SCOPE_ID_LEN + 4)]
            public String ScopeId;
            public int EnableRouting;
            public int EnableProxy;
            public int EnableDns;
        }


        enum NodeType_T { Broadcast = 1, PeerToPeer = 2, Mixed = 4, Hybrid = 8 }

        [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Ansi)]
        struct IPAddrString
        {
            public IntPtr NextPointer;

            [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 4 * 4)]
            public String IPAddressString;

            [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 4 * 4)]
            public String IPMaskString;
            public int Context;
        }



        private string Format(string filename, string arguments)
        {
            return "'" + filename +
                ((string.IsNullOrEmpty(arguments)) ? string.Empty : " " + arguments) +
                "'";
        }

        //DNS 설정 
        public void DNS_settings()
        {
            Trace.Write(DateTime.Now.ToString("[hh:mm:ss] ") + "DNS_settings 입장" + "\n");

            string dnsIp;
            string dnsIp2;

            GoEditUrl goEditUrl = GoEditUrl.GetInstance();

            SetDnsIp();

            dnsIp = goEditUrl.GetDnsIp();
            dnsIp2 = goEditUrl.GetDnsIp2();


            string[] asDNS = { dnsIp, dnsIp2 }; // DNS 서버는 최대 2개까지 설정할수 있음.

            string s, sArguments;
            while (true)
            {
                // ### Step 1. 현재 상태의 DNS 설정값을 얻어온다.
                bool[] abDNS = { false, false }; // 해당 DNS서버가 실제 설정되어있는지 체크
                Trace.Write(DateTime.Now.ToString("[hh:mm:ss] ") + "##### 설정된 DNS서버 목록을 요청" + "\n");
                ArrayList alInterface = GetSettingDNSList();

                for (int i = 0; i < 2; ++i)
                {
                    if ((i + 1) > alInterface.Count) // 필요한 설정개수보다 작은경우
                    {
                        if ("" == asDNS[i])
                        {
                            abDNS[i] = true;
                            continue;
                        }
                        break;
                    }

                    // 필요한 DNS 주소가 맞느지 설정된 값과 비교
                    if (alInterface[i].ToString() == asDNS[i])
                    {
                        abDNS[i] = true;
                    }
                }

                if ((!abDNS[0]) || (!abDNS[1])) // 필요한 DNS가 설정되어 있지 않은경우
                {
                    Trace.Write(DateTime.Now.ToString("[hh:mm:ss] ") + "##### 요청 DNS서버 목록과 불일치하여 재설정 시도" + "\n");
                    // ### Step 2. PC에 등록된 Interface목록을 얻어온다.
                    s = ShellExternalExe("netsh", "interface ipv4 show interfaces");
                    alInterface = ParserUseInterface(s);
                    Trace.Write(DateTime.Now.ToString("[hh:mm:ss] ") + "##### 설치된 네트워크장치의 인터페이스 취득 : " + alInterface.Count + "개\n");
                    foreach (string sInterface in alInterface)
                    {
                        // ### Step 3. 얻어온 Interface들에게 DNS주소를 설정한다.

                        if ("" != asDNS[0])
                        {
                            Trace.Write(DateTime.Now.ToString("[hh:mm:ss] ") + "##### 네트워크장치[" + sInterface + "]의 첫번재 DNS설정 : " + asDNS[0] + "\n");
                            sArguments = String.Format("-c int ip set dns name=\"{0}\" source=static addr={1} register=PRIMARY", sInterface, asDNS[0]);
                            ShellExternalExe("netsh", sArguments);
                            Trace.Write(DateTime.Now.ToString("[hh:mm:ss] ") + "##### 네트워크장치[" + sInterface + "]의 첫번재 DNS설정 완료\n");
                        }


                        if ("" != asDNS[1])
                        {
                            Trace.Write(System.DateTime.Now.ToString("[hh:mm:ss] ") + "##### 네트워크장치[" + sInterface + "]의 두번재 DNS설정 : " + asDNS[1] + "\n");
                            sArguments = String.Format("-c int ip add dns name=\"{0}\" addr={1} index=2", sInterface, asDNS[1]);
                            ShellExternalExe("netsh", sArguments);
                            Trace.Write(System.DateTime.Now.ToString("[hh:mm:ss] ") + "##### 네트워크장치[" + sInterface + "]의 두번재 DNS설정 완료\n");
                        }
                    }
                }
                else
                {
                    Trace.Write(DateTime.Now.ToString("[hh:mm:ss] ") + "##### 요청 DNS서버 목록과 일치하여 Skip" + "\n");
                    break;
                }



                Trace.WriteLine(DateTime.Now.ToString("[hh:mm:ss] ") + "##### DNS_settings 종료" + "\n");
            }
        }

        //sql sp 에서 dns 값 가져오기
        private void SetDnsIp()
        {
            SqlConnection conn = null;


            try
            {
                if (DB_Con.DB_connect() == true)
                {
                    GoEditUrl goEditUrl = GoEditUrl.GetInstance();
                    string dnsIp;
                    string dnsIp2;

                    conn = DB_Con.dbConn;


                    using (SqlCommand comm = conn.CreateCommand())
                    {
                        comm.CommandText = "dbo.USP_SET_DNS_IP";
                        comm.CommandType = CommandType.StoredProcedure;
                        comm.Parameters.Add("@p_STORE_IDX", SqlDbType.Int).Value = goEditUrl.GetStoreIdx();
                        

                        SqlDataReader sr = comm.ExecuteReader();
                        Console.WriteLine("USP_SET_DNS_IP 시작 \r\n");
                        if (sr.HasRows)
                        {
                            while (sr.Read())
                            {
                                dnsIp = Convert.ToString(sr.GetValue(sr.GetOrdinal("DnsIp")));
                                dnsIp2 = Convert.ToString(sr.GetValue(sr.GetOrdinal("DnsIp2")));
                                goEditUrl.SetDnsIp(dnsIp);
                                goEditUrl.SetDnsIp2(dnsIp2);
                                Trace.WriteLine(dnsIp);
                                Trace.WriteLine(dnsIp2);
                            }
                        }
                    }

                    conn.Close();
                }
            }
            catch (Exception e)
            {
                Debug.WriteLine(e.Message);
            }
        }

        private string ShellExternalExe(string filename, string arguments = null)
        {
            var process = new Process();

            process.StartInfo.FileName = filename;
            if (!string.IsNullOrEmpty(arguments))
            {
                process.StartInfo.Arguments = arguments;
            }

            process.StartInfo.CreateNoWindow = true;
            process.StartInfo.WindowStyle = ProcessWindowStyle.Hidden;
            process.StartInfo.UseShellExecute = false;

            process.StartInfo.RedirectStandardError = true;
            process.StartInfo.RedirectStandardOutput = true;
            var stdOutput = new StringBuilder();
            process.OutputDataReceived += (sender, args) => stdOutput.AppendLine(args.Data); // Use AppendLine rather than Append since args.Data is one line of output, not including the newline character.

            string stdError = null;
            try
            {
                process.Start();
                process.BeginOutputReadLine();
                stdError = process.StandardError.ReadToEnd();
                process.WaitForExit();
            }
            catch (Exception e)
            {
                throw new Exception("OS error while executing " + Format(filename, arguments) + ": " + e.Message, e);
            }

            try
            {
                if (process.ExitCode == 0)
                {
                    return stdOutput.ToString();
                }
                else
                {
                    var message = new StringBuilder();

                    if (!string.IsNullOrEmpty(stdError))
                    {
                        message.AppendLine(stdError);
                    }

                    if (stdOutput.Length != 0)
                    {
                        message.AppendLine("Std output:");
                        message.AppendLine(stdOutput.ToString());
                    }

                    throw new Exception(Format(filename, arguments) + " finished with exit code = " + process.ExitCode + ": " + message);
                }
            }
            catch (Exception e)
            {
                Debug.WriteLine("에라");
                throw new Exception("OS error while executing " + Format(filename, arguments) + ": " + e.Message, e);
            }
        }

        public ArrayList GetSettingDNSList()
        {
            int iSize = 0;
            int iRet = GetNetworkParams(null, ref iSize);
            ArrayList ips = new ArrayList();

            try
            {

                if ((iRet != ERROR_SUCCESS) && (iRet != ERROR_BUFFER_OVERFLOW))
                    throw new Exception("Error invoking GetNetworkParams() : " + iRet);

                Byte[] buffer = new Byte[iSize];
                iRet = GetNetworkParams(buffer, ref iSize);
                if (iRet != ERROR_SUCCESS)
                    throw new Exception("Error invoking GetNetworkParams() " + iRet);

                FixedInfo PFixedInfo = new FixedInfo();
                ByteArray_To_FixedInfo(ref PFixedInfo, buffer, Marshal.SizeOf(PFixedInfo));

                string sHostname = PFixedInfo.HostName;
                string sDomainname = PFixedInfo.DomainName;


                //ips.Add(IPAddress.Parse(PFixedInfo.DnsServerList.IPAddressString));
                ips.Add(PFixedInfo.DnsServerList.IPAddressString);

                IPAddrString ListItem = new IPAddrString();

                IntPtr ListNext = new IntPtr();

                ListNext = PFixedInfo.DnsServerList.NextPointer;

                while (ListNext.ToInt64() != 0)
                {
                    IntPtr_To_IPAddrString(ref ListItem, ListNext, Marshal.SizeOf(ListItem));
                    //ips.Add(IPAddress.Parse(ListItem.IPAddressString));
                    ips.Add(ListItem.IPAddressString);
                    ListNext = ListItem.NextPointer;
                }

                //IPAddress[] dnsServers = (IPAddress[])ips.ToArray(typeof(IPAddress));
            }
            catch
            {

            }
            return ips;
        }


        private ArrayList ParserUseInterface(string s)
        {
            ArrayList alInterface = new ArrayList();
            string sDevice;
            while (true)
            {
                int iPos = s.IndexOf("\n");
                if (-1 == iPos) break;

                sDevice = Left(s, iPos);
                int iPos2 = sDevice.IndexOf(" connected");
                if (-1 != iPos2)
                {
                    sDevice = Mid(sDevice, iPos2 + 11);
                    sDevice = sDevice.Replace("\r", "");
                    sDevice = sDevice.Replace("\n", "");
                    sDevice = sDevice.Trim();

                    alInterface.Add(sDevice);
                    Trace.Write(sDevice + "\n");
                }
                s = Mid(s, iPos + 2);
            }
            return alInterface;
        }

        public string Mid(string target, int start)
        {
            if (start <= target.Length)
            {
                return target.Substring(start - 1);
            }
            return string.Empty;
        }

        public string Left(string target, int length)
        {
            if (length <= target.Length)
            {
                return target.Substring(0, length);
            }
            return target;
        }

    }
}
