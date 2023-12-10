using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Diagnostics;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Xml;

namespace JoyGreen_PF
{
    public class DB_Con
    {

        public static string DB_SV = "";

        public static string cliVersion = "";
        public static string DB_Name = "";
        public static SqlConnection dbConn;
        public static string LICENSE_STATE = "";


        public static bool DB_connect()
        {
            Boolean conSata = false;    //DB 접속 성공여부

            try
            {
                XmlDocument xml = new XmlDocument();

                string url = Application.StartupPath + $@"\Xml\DB_Xml.xml";
                xml.Load(url);

                XmlElement KeyList = xml.DocumentElement;
                XmlNodeList xmlList = xml.SelectNodes("/CONFIG/DB_INFO");

                foreach (XmlNode node1 in xmlList)
                {
                    DB_Con.DB_SV = node1["DATA_SOURCE"].InnerText;
                    DB_Con.DB_Name = node1["DATABASE"].InnerText;
                }

                string DBCon = "server = " + DB_SV + "; uid = joygreen_pf; pwd = joygreen; database = " + DB_Name + ";";

                dbConn = new SqlConnection(DBCon);
                dbConn.Open();


                conSata = true;

                return conSata;
            }
            catch (SqlException e)
            {
                conSata = false;
                
                return conSata;
            }
        }

        //문자열에 대한 MD5 해시를 생성
        public static string MD5HashFunc(string str)
        {
            StringBuilder MD5Str = new StringBuilder(); //StringBuilder를 사용하여 문자열을 효율적으로 조작하기 위한 MD5Str 객체를 생성
            byte[] byteArr = Encoding.ASCII.GetBytes(str); //주어진 문자열 str을 ASCII 인코딩을 사용하여 바이트 배열로 변환
            byte[] resultArr = (new MD5CryptoServiceProvider()).ComputeHash(byteArr); //MD5CryptoServiceProvider를 사용하여 바이트 배열에 대한 MD5 해시를 계산하고, 결과를 resultArr에 저장

            for (int cnti = 0; cnti < resultArr.Length; cnti++)
            {
                MD5Str.Append(resultArr[cnti].ToString("X2"));
            }
            return MD5Str.ToString();
        }


        private static string GetDateTime()
        {
            DateTime NowDate = DateTime.Now;
            return NowDate.ToString("yyyy-MM-dd HH:mm:ss") + ":" + NowDate.Millisecond.ToString("000");
        }

    }

    public class GoEditUrl
    {
        private string p_FILE;
        private string p_FRAG;
        private int p_PORT = 0;
        private string tempUrl;
        private int storeIdx;
        private int myComNo;
        private string currentUrl;
        private int blockBrowser;
        private string blockUrl;
        private int blockYN;
        private string movingUrl;
        private int blockCrYn = 0;
        private int blockFfYn = 0;
        private int blockTimCr = 0;
        private string dnsIp;
        private string dnsIp2;
        public int PasswordCount_ip { get; set; } = 0;
        public int PasswordCount_close { get; set; } = 0;
        public string clientIp { get; set; } = "";
        public string macAddress { get; set; } = "";
        public int dbUpdateCnt { get; set; } = 0;
        public string 정상처리YN { get; set; } = "N";
        public string TrayIconYn { get; set; } = "N";
        //public string INTENALIPYN { get; set; } = "N";
        public string ExternalIP { get; set; } = "";
        public string 검색어 { get; set; } = "";
        public List<string> 검색어List = new List<string>();
        public List<string> 검색어List_file = new List<string>();
        public List<string> 검색어List_VPN = new List<string>();
        public DateTime dbUpdateDate { get; set; }
        public string store_name { get; set; } = "";
        private static GoEditUrl goEditUrl;

        private GoEditUrl()
        {
            p_FILE = string.Empty;
            p_FRAG = string.Empty;
            p_PORT = 0;
            tempUrl = string.Empty;
            //p_PLIAN_Cr_URL = string.Empty;
            storeIdx = 0;
            myComNo = 0;
            currentUrl = string.Empty;
            blockBrowser = 0;
            blockUrl = string.Empty;
            blockYN = 0;
            movingUrl = $@"https://defender.joygreen.kr/warning2022.asp";
            blockCrYn = 0;
            blockFfYn = 0;
            blockTimCr = 0;
            dnsIp = string.Empty;
            dnsIp2 = string.Empty;
        }


        public static GoEditUrl GetInstance()
        {
            if (goEditUrl == null) goEditUrl = new GoEditUrl();
            return goEditUrl;
        }


        internal void SetDnsIp2(string val)
        {
            this.dnsIp2 = val;
        }
        internal string GetDnsIp2()
        {
            return dnsIp2;
        }

        internal void SetDnsIp(string val)
        {
            this.dnsIp = val;
        }
        internal string GetDnsIp()
        {
            return dnsIp;
        }

        internal void SetBlockTimCr(int val)
        {
            this.blockTimCr = val;
        }
        internal int GetBlockTimCr()
        {
            return blockTimCr;
        }
        internal void SetBlockFfYn(int val)
        {
            this.blockFfYn = val;
        }
        internal int GetBlockFfYn()
        {
            return blockFfYn;
        }
        internal void SetBlockCrYn(int val)
        {
            this.blockCrYn = val;
        }
        internal int GetBlockCrYn()
        {
            return blockCrYn;
        }
        internal void SetMovingUrl(string val)
        {
            this.movingUrl = val;
        }
        internal string GetMovingUrl()
        {
            return movingUrl;
        }
        internal void SetBlockYN(int val)
        {
            this.blockYN = val;
        }
        internal int GetBlockYN()
        {
            return blockYN;
        }
        internal void SetBlockUrl(string val)
        {
            this.blockUrl = val;
        }
        internal string GetBlockUrl()
        {
            return blockUrl;
        }
        internal void SetBlockBrowser(int val)
        {
            this.blockBrowser = val;
        }
        internal int GetBlockBrowser()
        {
            return blockBrowser;
        }
        internal void SetCurrentUrl(string val)
        {
            this.currentUrl = val;
        }
        internal string GetCurrentUrl()
        {
            return currentUrl;
        }
        internal void SetmyComNo(int val)
        {
            this.myComNo = val;
        }
        internal int GetmyComNo()
        {
            return myComNo;
        }
        internal void SetStoreIdx(int val)
        {
            this.storeIdx = val;
        }

        internal int GetStoreIdx()
        {
            return storeIdx;
        }

        internal void SetP_FILE(string val)
        {
            this.p_FILE = val;
        }
        internal string GetP_FILE()
        {
            return p_FILE;
        }
        internal void SetP_FRAG(string val)
        {
            this.p_FRAG = val;
        }

        internal string GetP_FRAG()
        {
            return p_FRAG;
        }
        internal void SetP_PORT(int val)
        {
            this.p_PORT = val;
        }

        internal int GetP_PORT()
        {
            return p_PORT;
        }
        internal void SetTempUrl(string val)
        {
            this.tempUrl = val;
        }

        internal string GetTempUrl()
        {
            return tempUrl;
        }

        public string EditUrl(String url, int checkHttpsYn, string portNo)
        {
            /////url을 규격을 맞춘다. 
            String editUrl = "";
            String finalEditUrl = "";
            String firstUrl = "";
            String httpUrl = "";

            int my_p_Port = 0;
            string my_p_FILE = "/";
            string my_p_FRAG = "/";
            int i = 0;



            editUrl = url.Trim();
            Debug.WriteLine("editurl: " + editUrl);
            string[] splitUrls = editUrl.Split('/');

            foreach (string splitUrl in splitUrls)
            {
                if (i == 0)
                {
                    httpUrl = splitUrl;
                    firstUrl = splitUrl;
                    editUrl = splitUrl;
                }
                else if (i == 2)
                {
                    if (httpUrl == "https:")
                    {
                        editUrl = splitUrl;
                    }
                    else if (httpUrl == "http:")
                    {
                        editUrl = splitUrl;
                    }
                    else
                    {
                        editUrl = firstUrl;
                    }
                }
                else if (i > 2)
                {
                    if (i == splitUrls.Length - 1)
                    {
                        my_p_FRAG += splitUrl;
                    }
                    else
                    {
                        my_p_FRAG += splitUrl + "/";
                    }
                }
                i = i + 1;
            }

            i = 0;
            if (my_p_FRAG.Contains("?"))
            {
                string[] splitUrls2 = my_p_FRAG.Split('?');
                foreach (string splitUrl2 in splitUrls2)
                {
                    if (i == 0)
                    {
                        my_p_FILE = splitUrl2;
                    }
                    i = i + 1;
                }
            }
            else
            {
                my_p_FILE = my_p_FRAG;
            }
            p_FILE = my_p_FILE;
            p_FRAG = my_p_FRAG;

            ///@앞부분을 없엔다.
            i = 0;
            if (editUrl.Contains("@"))
            {
                string[] splitUrls2 = editUrl.Split('@');
                foreach (string splitUrl2 in splitUrls2)
                {
                    if (i == 1)
                    {
                        editUrl = splitUrl2;
                    }
                    i = i + 1;
                }
            }

            ///포트가 없으면 포트를 넣어준다
            if (editUrl.Contains(":"))
            {
                my_p_Port = 0;
            }
            else
            {
                editUrl += ":" + portNo;
                my_p_Port = int.Parse(portNo);
            }
            p_PORT = my_p_Port;
            tempUrl = editUrl;

            if (checkHttpsYn == 0)
                editUrl = "http://" + editUrl;
            else
                editUrl = "https://" + editUrl;

            ///www.을 없엔다./////////////////
            finalEditUrl = editUrl.Replace("http://www.", "http://");
            finalEditUrl = finalEditUrl.Replace("https://www.", "http://");
            ///소문자로 변경 /////////////
            finalEditUrl = finalEditUrl.ToLower();

            return finalEditUrl;
        }


    }


}
