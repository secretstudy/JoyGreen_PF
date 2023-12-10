using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Data.SqlClient;
using System.Diagnostics;
using System.Drawing;
using System.Linq;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Automation;
using System.Windows.Forms;
using Accessibility;

namespace JoyGreen_PF
{


    public partial class Form1 : Form
    {
        private System.Timers.Timer chromeTimer;

        //로그인 상태 변수 선언, 비 로그인 상태는 0
        public static int login_status = 0;


        internal const int GW_HWNDNEXT = 2;

        Set_DNS Set_DNS = new Set_DNS();

        string chromeUrl = string.Empty;
        string chromeHandle = string.Empty;
        private int chromeToggle = 0;

        string fireFoxUrl = string.Empty;

        // Object & Child ID 관리
        const uint DEFAULT_CHROME_OBJECT_ID = 4294967292;
        const uint DEFAULT_CHROME_CHILD_ID = 4294967295; 
        const uint DEFAULT_CHROME_CHILD_STEP = 100; 

        const int VK_RETURN = 0x0D;
        const int MK_LBUTTON = 0x0001;
        const int WM_KEYDOWN = 0x100;
        const int WM_KEYUP = 0x101;

        // Chrome Object 관리                
        uint m_uiLastChromeChildID = DEFAULT_CHROME_CHILD_ID;
        public struct ChromeObject
        {
            public IntPtr hWnd;
            public uint uiChildID;
        };
        public ArrayList ChromeObjectArray = new ArrayList();

        // 추후 URL이동을 위한 Object정보
        IAccessible chromeiAccessible = null;
        object chromeChildId = null;

        ArrayList urlListCrNow = new ArrayList();

        
        #region APIs

        [DllImport("oleacc.dll")]
        public static extern uint WindowFromAccessibleObject(IAccessible pacc, ref IntPtr phwnd);

        [DllImport("oleacc.dll")]
        public static extern IntPtr AccessibleObjectFromEvent(IntPtr hwnd, uint dwObjectID, uint dwChildID,
            out IAccessible ppacc, [MarshalAs(UnmanagedType.Struct)] out object pvarChild);


        [DllImport("user32.dll", SetLastError = true)]
        public static extern IntPtr FindWindow(string lpClassName, string lpWindowName);

        [DllImport("user32.dll")]
        public static extern int GetClassName(IntPtr hWnd, StringBuilder lpClassName, int nMaxCount);

        [DllImport("User32.dll")]
        public static extern IntPtr GetParent(IntPtr hwnd);

        [DllImport("user32.dll")]
        static extern uint GetWindowThreadProcessId(IntPtr hWnd, ref IntPtr ProcessId);

        [DllImport("user32.dll")]
        public static extern IntPtr GetWindow(IntPtr hWnd, uint uCmd);

        [DllImport("user32.dll", CharSet = CharSet.Auto, SetLastError = true)]
        static extern int GetWindowText(IntPtr hWnd, StringBuilder lpString, int nMaxCount);

        [DllImport("User32.dll", CharSet = CharSet.Auto)]
        public extern static bool PostMessage(IntPtr hwnd, int msg, IntPtr wParam, IntPtr lParam);

        [DllImport("kernel32")]
        public static extern IntPtr CreateToolhelp32Snapshot(Int32 dwFlags, Int32 th32ProcessID);
        [DllImport("kernel32")]
        public static extern void CloseHandle(IntPtr hObject);

        [DllImport("kernel32")]
        public static extern Int32 Process32First(IntPtr hSnapshot, ref PROCESSENTRY32 pe32);

        [DllImport("kernel32")]
        public static extern Int32 Process32Next(IntPtr hSnapshot, ref PROCESSENTRY32 pe32);

        public const Int32 MAX_PATH = 260;
        public const Int32 TH32CS_SNAPPROCESS = 2;

        #endregion


        [StructLayout(LayoutKind.Sequential)]
        public struct PROCESSENTRY32
        {
            public Int32 dwSize;
            public Int32 cntUsage;
            public Int32 th32ProcessID;
            public IntPtr th32DefaultHeapID;
            public Int32 th32ModuleID;
            public Int32 cntThreads;
            public Int32 th32ParentProcessID;
            public Int32 pcPriClassBase;
            public Int32 dwFlags;
            [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 260)]
            public String szExeFile;

            public static Int32 Size
            {
                get { return Marshal.SizeOf(typeof(PROCESSENTRY32)); }
            }
        }

        public List<PROCESSENTRY32> EnumProcesses()
        {
            IntPtr hSnapshot = CreateToolhelp32Snapshot(TH32CS_SNAPPROCESS, 0);
            if (hSnapshot == IntPtr.Zero)
                return null;

            PROCESSENTRY32 pe32 = new PROCESSENTRY32();
            pe32.dwSize = PROCESSENTRY32.Size;
            if (Process32First(hSnapshot, ref pe32) == 0)
            {
                CloseHandle(hSnapshot);
                return null;
            }

            List<PROCESSENTRY32> lstProcesses = new List<PROCESSENTRY32>();
            do
            {
                lstProcesses.Add(pe32);
            } while (Process32Next(hSnapshot, ref pe32) != 0);

            CloseHandle(hSnapshot);
            return lstProcesses;
        }

        public string GetSafeAccNameFromAccessibleObj(Accessibility.IAccessible iAccessible, object oChild)
        {
            string sAccName = null;
            try
            {

                sAccName = iAccessible.get_accName(oChild);
            }
            catch (Exception e)
            {
                Trace.WriteLine(e.Message);
                return null;
            }
            return sAccName;
        }

        public string GetSafeAccValueFromAccessibleObj(Accessibility.IAccessible iAccessible, object oChild)
        {
            string sAccName = null;
            try
            {
                sAccName = iAccessible.get_accValue(oChild);
            }
            catch (Exception e)
            {
                Trace.WriteLine(e.Message);
                return null;
            }
            return sAccName;
        }

        public Form1()
        {
            InitializeComponent();
            Trayicon.ContextMenuStrip = Context_TrayIcon;
            Trayicon.Visible = true;

            this.TopMost = true;// 창 안내려 가게 고정하기

            SetContextMenuStrip();
        }

        private void joinlabel_Click(object sender, EventArgs e)
        {
            this.Hide();//기존 로그인 창 숨기기

            Join join = new Join();
            join.Show();
        }

        private void read_pw_CheckedChanged(object sender, EventArgs e)
        {
            //체크시 비밀번호를 보이게 한다
            if (read_pw.Checked == true)
            {
                if (loginPwd.Text == "************")
                    loginPwd.Text = "";

                if (loginPwd.Text == "비밀번호를 입력해주세요")
                    loginPwd.Text = "";

                // default(char)는 기본값으로 설정한다는 기능
                // 기본값은 빈문자열을 나타내며 이경우 평문으로 표시된다.
                // 즉 입력한 비밀번호가 숨겨지지 않고 표시된다.
                loginPwd.PasswordChar = default(char);
            }

            //체크 안하면 *로 표시한다.
            else
                loginPwd.PasswordChar = '*';
        }//private void read_pw_CheckedChanged(object sender, EventArgs e)
        private void loginID_MouseDown(object sender, MouseEventArgs e)
        {
            if (loginID.Text == "아이디를 입력해주세요")
                loginID.Text = "";

            loginID.ForeColor = Color.Black;
        }

        private void loginPwd_MouseDown(object sender, MouseEventArgs e)
        {
            //텍스트가 비밀번호 입력해주세요 OR * 일 경우 마우스 클릭시 텍스트 초기화
            if (loginPwd.Text == "***********" || loginPwd.Text == "비밀번호를 입력해주세요")
                loginPwd.Text = "";
            //txtPwd.Text = "";
            loginPwd.ForeColor = Color.Black;
        }

        private void login_btn_Click(object sender, EventArgs e)
        {
            try
            {
                SqlConnection connection = new SqlConnection("Server = DESKTOP-97LBKU6;Database=JoyGreen_PF;Uid=joygreen_pf;Pwd=joygreen;");
                connection.Open();


                string loginid = loginID.Text;
                string loginpwd = loginPwd.Text;

                string selectQuery = "SELECT * FROM TBL_JG_STORE WHERE id = \'" + loginid + "\' ";
                SqlCommand Selectcommand = new SqlCommand(selectQuery, connection);

                SqlDataReader userAccount = Selectcommand.ExecuteReader();

                while (userAccount.Read())
                {
                    if (loginid == (string)userAccount["id"] && loginpwd == (string)userAccount["pw"])
                    {
                        login_status = 1;//로그인 성공 상태로 변경
                    }
                }
                connection.Close();

                if (login_status == 1)//로그인 성공시
                {
                    this.Hide();

                    SetContextMenuStrip();

                    Set_DNS.DNS_settings();

                    if (Process.GetProcessesByName("chrome").Length > 0)
                    {
                        Trace.WriteLine("크롬 차단시작\n");
                        SearchChrome();
                        blockChrome();
                        Trace.WriteLine("크롬 차단끝\n");
                    }

                    if (Process.GetProcessesByName("firefox").Length > 0 )
                    {
                        Trace.WriteLine("파이어폭스 시작\n");
                        SearchFireFox();
                    }

                }
                else
                {
                    MessageBox.Show("회원 정보를 확인해 주세요.");
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }//private void login_btn_Click(object sender, EventArgs e)

        private void label2_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            ContextMenuStrip contextMenu = new ContextMenuStrip();

        }

        //트레이아이콘 리스트 생성
        public ContextMenuStrip ContextCreate()
        {
            ContextMenuStrip contextMenu = new ContextMenuStrip();

            if (login_status == 1) // 로그인 성공시
            {
                this.Hide();

                // 어셈블리 정보를 가져옵니다.
                Assembly assembly = Assembly.GetExecutingAssembly();
                AssemblyName assemblyName = assembly.GetName();

                ToolStripMenuItem loginItem = new ToolStripMenuItem();
                loginItem.Text = "로그인";
                loginItem.Click += new EventHandler(Login_Click);
                contextMenu.Items.Add(loginItem);       
                

                contextMenu.Items.Add("exit");
                contextMenu.Items.Add("클라이언트 버전: " + assemblyName.Version.ToString());
                contextMenu.Items.Add("매장정보(좌석번호): " + loginID.Text + "(" + loginComNo.Text + ")");
            }
            else if (login_status == 0)
            {
                // 어셈블리 정보를 가져옵니다.
                Assembly assembly = Assembly.GetExecutingAssembly();
                AssemblyName assemblyName = assembly.GetName();

                ToolStripMenuItem loginItem = new ToolStripMenuItem();
                loginItem.Text = "로그인";
                loginItem.Click += new EventHandler(Login_Click);
                contextMenu.Items.Add(loginItem);
                contextMenu.Items.Add("exit");
                contextMenu.Items.Add("클라이언트 버전: " + assemblyName.Version.ToString());
                contextMenu.Items.Add("매장정보(좌석번호): 비로그인");
            }

            return contextMenu; // ContextMenuStrip 반환
        }//public ContextMenuStrip ContextCreate()
        private void Login_Click(object sender, EventArgs e)
        {
            // "로그인" 메뉴 클릭 시 Form1 보여줌
            Form1 form1 = new Form1();
            form1.Show();
        }


        private void SetContextMenuStrip()
        {
            ContextMenuStrip contextMenuStrip = ContextCreate(); // 위에서 작성한 함수 호출

            if (contextMenuStrip != null)
            {
                Trayicon.ContextMenuStrip = contextMenuStrip; // NotifyIcon과 ContextMenuStrip 연결
            }
        }

        private void loginComNo_MouseDown(object sender, MouseEventArgs e)
        {
            loginComNo.Text = "";
            loginComNo.ForeColor = Color.Black;
        }

        private void Trayicon_MouseClick(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Left) // 마우스 왼쪽 버튼 클릭
            {
                ContextMenuStrip menu = ContextCreate();
                menu.Show(Cursor.Position); // 커서 위치에 메뉴 표시
            }
        }


        // 윈도우 핸들로 프로세스 아이디 얻기   
        public IntPtr ProcIDFromWnd(IntPtr hwnd)
        {
            IntPtr ipPorc = IntPtr.Zero;
            uint uiThread = GetWindowThreadProcessId(hwnd, ref ipPorc);
            return ipPorc;
        }

        public int GetSafeCompareToString(string sSrc, string sCompare)
        {
            int iRet = -1;
            if (null != sSrc)
            {
                try
                {
                    iRet = sSrc.CompareTo(sCompare);
                }
                catch (Exception e)
                {
                    Trace.WriteLine(e.Message);
                    return -1;
                }
            }
            return iRet;
        }


        public bool GetSafeClassName(IntPtr hWnd, StringBuilder lpClassName, int iMaxSize)
        {
            try
            {
                GetClassName(hWnd, lpClassName, iMaxSize);
            }
            catch (Exception e)
            {
                Trace.WriteLine(e.Message);
                return false;
            }
            return true;
        }

        public bool GetSafeWindowText(IntPtr hWnd, StringBuilder lpWindowText, int iMaxSize)
        {
            try
            {
                GetWindowText(hWnd, lpWindowText, iMaxSize);
            }
            catch (Exception e)
            {
                Trace.WriteLine(e.Message);
                return false;
            }
            return true;
        }

        public int GetSafeFindToString(string sSrc, string sFind)
        {
            int iRet = -1;
            if (null != sSrc)
            {
                try
                {
                    iRet = sSrc.IndexOf(sFind);
                }
                catch (Exception e)
                {
                    Trace.WriteLine(e.Message);
                    return -1;
                }
            }
            return iRet;
        }

        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        ///// 크롬 검색 및 관리 ////////////////////////////////////////////////////////////////////////////////////////////////
        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////



        private void SearchChrome()
        {
            List<PROCESSENTRY32> processes = EnumProcesses();

            if (processes != null)
            {
                foreach (PROCESSENTRY32 pe32 in processes)
                {
                    if (0 == pe32.szExeFile.CompareTo("chrome.exe"))// Ver 77.0.3865.90
                    {
                        IntPtr hChrome = GetWinHandleSupportOnlyChrome((IntPtr)pe32.th32ProcessID);

                        if (IntPtr.Zero != hChrome)
                        {
                            chromeHandle = hChrome.ToString();
                            ChromeUrl();
                        }
                    }
                }
            }
            else
            {
                Console.WriteLine("목록 로드 실패");
            }
        }

        // 프로세스 아이디로 윈도우 핸들 얻기(크롬전용) 
        private IntPtr GetWinHandleSupportOnlyChrome(IntPtr pid)  // Ver 77.0.3865.90
        {
            IntPtr tempHwnd = FindWindow(null, null); // 최상위 윈도우 핸들 찾기   

            while (tempHwnd != IntPtr.Zero)
            {
                if (GetParent(tempHwnd) == IntPtr.Zero) // 최상위 핸들인지 체크, 버튼 등도 핸들을 가질 수 있으므로 무시하기 위해   
                {
                    if (pid == ProcIDFromWnd(tempHwnd))
                    {
                        //OutputDebugString("크롬 >> PID : " + pid + "hWnd : " + tempHwnd + "\r\n");
                        StringBuilder lpClassName = new StringBuilder(1024); // 클레스명 비교
                        if (true == GetSafeClassName(tempHwnd, lpClassName, lpClassName.Capacity))
                        {
                            if (0 == GetSafeCompareToString(lpClassName.ToString(), "Chrome_WidgetWin_1"))
                            {
                                StringBuilder lpWindowText = new StringBuilder(1024); // 윈도우 타이틀 비교
                                if (true == GetSafeWindowText(tempHwnd, lpWindowText, lpWindowText.Capacity))
                                {
                                    if (-1 != GetSafeFindToString(lpWindowText.ToString(), " - Chrome"))
                                    {
                                        return tempHwnd;
                                    }
                                }
                            }
                        }
                    }
                }
                tempHwnd = GetWindow(tempHwnd, GW_HWNDNEXT); // 다음 윈도우 핸들 찾기   
            }
            return IntPtr.Zero;
        }

        private void ChromeUrl()
        {
            string url = string.Empty;

            if (0 >= chromeHandle.Length) return;

            int iHwnd = Int32.Parse(chromeHandle);
            IntPtr hWnd = (IntPtr)iHwnd;
            if (IntPtr.Zero == hWnd) return;

            // 등록된 Chrome Object 유효성 판단
            try
            {
                CheckChromeObjects();
            }
            catch (Exception)
            {
                Debug.WriteLine("크롬 검색 실패");
            };

            // 요청된 Chrome hWnd 검색(이전 목록에서)
            uint uiChild = GetChildIDFromChromeObject(hWnd);
            GetURLChromeBrowser(hWnd, uiChild);
        }

        // 등록된 ChromeObject들의 유효성을 판단한다.
        public void CheckChromeObjects()
        {
            int iCount = ChromeObjectArray.Count;
            for (int i = 0; i < iCount; ++i)
            {
                ChromeObject co = (ChromeObject)ChromeObjectArray[i];
                if (IntPtr.Zero == co.hWnd) // 유효하지 않은 hWnd 제거
                {
                    ChromeObjectArray.RemoveAt(i);
                    iCount--;
                    i--;
                    continue;
                }

                Accessibility.IAccessible iAccessible;
                object ChildId;

                IntPtr handler = IntPtr.Zero;
                handler = AccessibleObjectFromEvent(co.hWnd, DEFAULT_CHROME_OBJECT_ID, co.uiChildID, out iAccessible, out ChildId);
                if (null == ChildId) // 종료된 창으로 ChildID를 제거
                {
                    ChromeObjectArray.RemoveAt(i);
                    iCount--;
                    i--;
                    continue;
                }
                if (-1 >= (int)ChildId) // 유효하지 않은 ChildID 제거
                {
                    ChromeObjectArray.RemoveAt(i);
                    iCount--;
                    i--;
                    continue;
                }

                StringBuilder lpClassName = new StringBuilder();
                GetClassName(co.hWnd, lpClassName, 100);

                int iRet = lpClassName.ToString().CompareTo("Chrome_WidgetWin_1");
                if (0 != iRet) // 유효하지 않은 ClassName 제거
                {
                    ChromeObjectArray.RemoveAt(i);
                    iCount--;
                    i--;
                    continue;
                }
                else if (0 == iRet)
                {
                    string sAccName = iAccessible.get_accName(ChildId);
                    iRet = sAccName.CompareTo("주소창 및 검색창"); // 한글 버젼
                    if (0 != iRet)
                    {
                        iRet = sAccName.CompareTo("Address and search bar"); // 영문 버젼
                    }

                    if (0 != iRet) // 유효하지 않은 Accessible Name 제거
                    {
                        ChromeObjectArray.RemoveAt(i);
                        iCount--;
                        i--;
                        continue;
                    }
                    else if (0 == iRet)
                    {
                        // 유효한 Object
                        //string sValue = iAccessible.get_accValue(ChildId);
                        //System.Diagnostics.Trace.Write(co.hWnd + ", " + lpClassName + ", " + sAccName + ", " + sValue + "\r\n");
                    }
                }
            }

            // 정리후 Object Count 확인
            iCount = ChromeObjectArray.Count;
            if (0 >= iCount)
            {
                // 모든 Chrome가 종료되었을경우 새창은 Default값으로 시작됨.
                m_uiLastChromeChildID = DEFAULT_CHROME_CHILD_ID;
            }
        }//public void CheckChromeObjects()

        // 등록된 ChromeObject 목록에서 검색된 hWnd를 검색한다.
        private uint GetChildIDFromChromeObject(IntPtr hWnd)
        {
            int iCount = ChromeObjectArray.Count;
            for (int i = 0; i < iCount; ++i)
            {
                ChromeObject co = (ChromeObject)ChromeObjectArray[i];
                if (IntPtr.Zero == co.hWnd) continue;

                if (co.hWnd == hWnd)
                {
                    return co.uiChildID;
                }
            }
            // 검색된 ID가 없을경우 마지막 검색 ID를 리턴하여 그후 부터 검색할 수 있도록 한다.
            return m_uiLastChromeChildID;// DEFAULT_CHROME_CHILD_ID;
        }

        // 핸들과 ID를 가지고 URL을 얻는다.
        private bool GetURLChromeBrowser(IntPtr hWnd, uint uiID, bool bFixID = false)
        {
            IAccessible iAccessible;
            object ChildId;

            int iSearchCount = 0;
            int iLimit = bFixID ? 1 : 100000; // 10000회까지만 검색

            // 최대값 비교
            // 1회이상 검색된 Item의 경우 Array에 등록되어 있기때문에 증가하며 추가 검색은 하지 않는다. 
            if (DEFAULT_CHROME_CHILD_ID <= uiID)
            {
                // Chrome 최초 실행시
                uiID = DEFAULT_CHROME_CHILD_ID;
            }
            else if (false == bFixID)
            {
                // 이미 검색된 아이디는 검색할 필요없으므로 1회에 한하여 검색해본다.
                bool bRet = GetURLChromeBrowser(hWnd, uiID, true);
                if (true == bRet)
                {
                    return true;
                }
            }

            uint uiGapValue = bFixID ? 1 : DEFAULT_CHROME_CHILD_STEP; // Child값이 -, 0가 혼합되어 있으나 약 410단계 사이에 혼합되어 있음, 100Step를 가짐
            while (iSearchCount < iLimit)
            {
                iSearchCount++;
                IntPtr handler = IntPtr.Zero;
                handler = AccessibleObjectFromEvent(hWnd, DEFAULT_CHROME_OBJECT_ID, uiID, out iAccessible, out ChildId);
                if (null == ChildId)
                {                                       
                    uiID -= uiGapValue;
                    continue;
                }

                if (-1 >= (int)ChildId)
                {
                    uiID -= uiGapValue;
                    continue;
                }
                else if (DEFAULT_CHROME_CHILD_STEP == uiGapValue) // ChildId가 정상값(0)이 검색된 최초 아이디시...
                {
                    uiID += uiGapValue; // 검색시작범위를 재정의 한다.
                    uiGapValue = 1; // 검색 Step를 줄여준다.

                    iLimit = 500; // 검색범위를 줄여준다.
                    iSearchCount = 0; // 검색범위 초기화에 따른 횟수 초기화
                    continue;
                }

                StringBuilder lpClassName = new StringBuilder(1024);
                if (false == GetSafeClassName(hWnd, lpClassName, lpClassName.Capacity))
                {
                    uiID -= uiGapValue;
                    continue;
                }

                int iRet = GetSafeCompareToString(lpClassName.ToString(), "Chrome_WidgetWin_1");
                if (0 != iRet)
                {
                    uiID -= uiGapValue;
                    continue;
                }
                else if (0 == iRet)
                {
                    string sAccName = GetSafeAccNameFromAccessibleObj(iAccessible, ChildId);
                    iRet = GetSafeCompareToString(sAccName, "주소창 및 검색창"); // 한글 버젼
                    if (0 != iRet)
                    {
                        iRet = GetSafeCompareToString(sAccName, "Address and search bar"); // 영문 버젼
                    }

                    if (0 != iRet)
                    {
                        uiID -= uiGapValue;
                        continue;
                    }
                    else if (0 == iRet)
                    {
                        string sValue = GetSafeAccValueFromAccessibleObj(iAccessible, ChildId);

                        // 검증된 Chrome Object를 등록한다.
                        AddChromeObject(hWnd, uiID);
                        
                        chromeUrl = sValue;
                        Trace.Write(hWnd + ", " + chromeUrl + "\r\n");

                        chromeiAccessible = iAccessible;
                        chromeChildId = ChildId;
                        return true;
                    }
                }
                break;
            }
            return false;
        }//private bool GetURLChromeBrowser(IntPtr hWnd, uint uiID, bool bFixID = false)

        private void AddChromeObject(IntPtr hWnd, uint uiChildID)
        {
            int iCount = ChromeObjectArray.Count;
            for (int i = 0; i < iCount; ++i)
            {
                ChromeObject co = (ChromeObject)ChromeObjectArray[i];
                if (IntPtr.Zero == co.hWnd) continue;

                if (co.hWnd == hWnd)
                {
                    return;
                }
            }

            // 미등록된 것으로 판단하여 신규 등록함.
            ChromeObject coNew = new ChromeObject();
            coNew.hWnd = hWnd;
            coNew.uiChildID = uiChildID;

            ChromeObjectArray.Add(coNew);

            // m_uiLastChildID 갱신
            if (m_uiLastChromeChildID > uiChildID)
            {
                m_uiLastChromeChildID = uiChildID;
            }
        }

        private void blockChrome()
        {
            int checkUrl = 0;
            string goNotifyUrl = string.Empty;

            string url = chromeUrl;

            if (url == null)
                return;//continue;
            if (url == "")
                return;// continue;
            if (url.Contains("https://defender.joygreen.kr/warning2022.asp"))
            {
                string[] result = url.Split(new string[] { $@"?" }, StringSplitOptions.None);
                string tempUrl = string.Empty;
                if (url.Length >= 2)
                {
                    tempUrl = result[1];
                    string[] result2 = tempUrl.Split(new string[] { $@"=" }, StringSplitOptions.None);
                    tempUrl = result2[1];
                }
                Thread.Sleep(1000);
                MovingChrome_http(tempUrl);
                return;// continue;
            }

            for (int i = 0; i <= urlListCrNow.Count - 1; i++)//한번 통과된것은 다시 검색하지 않는다.
            {
                if (urlListCrNow[i].Equals(url))
                {
                    checkUrl = 1;
                }
            }           
            MovingChrome_http(url);
        }//private void blockChrome()

        public void MovingChrome_http(string goNotifyUrl)
        {
            try
            {
                Thread.Sleep(100);
                if ((null != chromeiAccessible) && (null != chromeChildId))
                {
                    string sURL = $@"http://defender.joygreen.kr/warning2022.asp?key1=" + goNotifyUrl;
                    chromeiAccessible.set_accValue(chromeChildId, sURL);
                }
                else
                {
                    SendKeys.Send($@"%dhttp://defender.joygreen.kr/warning2022.asp?key1=" + goNotifyUrl);////0
                    Thread.Sleep(100);
                }
                Thread.Sleep(100);                
                SendKeys.Send("{ENTER}");///3
                SendKeys.Send("{ENTER}");///3
                Thread.Sleep(100);
                SendKeys.Send("{ENTER}");///3
                SendKeys.Send("{ENTER}");///3
                Thread.Sleep(100);
                SendKeys.SendWait("^l");////2
                SendKeys.Send("{ENTER}");///3                
                Thread.Sleep(100);
            }
            catch (Exception e)
            {
                Debug.WriteLine(e.Message);
            }
            finally
            {
            }
        }//public void MovingChrome_http(string goNotifyUrl)


        public void MovingChrome(string goNotifyUrl)
        {
            try
            {
                Thread.Sleep(100);

                if ((null != chromeiAccessible) && (null != chromeChildId))
                {
                    if (chromeToggle != 0)
                    {
                        SendKeys.Send("{f6}");////0
                        chromeToggle = 0;
                    }
                    else
                    {
                        chromeToggle = 1;
                    }
                    string sURL = $@"https://defender.joygreen.kr/warning2022.asp?key1=" + goNotifyUrl;
                    chromeiAccessible.set_accValue(chromeChildId, sURL);

                }
                else
                {
                    SendKeys.Send($@"%dhttps://defender.joygreen.kr/warning2022.asp?key1=" + goNotifyUrl);////0
                }
                Thread.Sleep(100);
                SendKeys.Send("{ENTER}");///3
                SendKeys.Send("{ENTER}");///3
                Thread.Sleep(100);
                SendKeys.SendWait("^l");////2

                SendKeys.Send("{ENTER}");///3
                SendKeys.Send("{ENTER}");///3
                SendKeys.Send("{f6}");////0
                Thread.Sleep(500);
                SendKeys.SendWait("^l");////2
            }
            catch (Exception e)
            {
                Debug.WriteLine(e.Message);
            }
            finally
            {
            }
        }//public void MovingChrome(string goNotifyUrl)        

        /////////////////////////////////////
        ///// 파이어폭스 검색 및 관리 ///////
        /////////////////////////////////////

        private string blockFireFoxYN()
        {           
            int checkUrl = 0;
            string goNotifyUrl = string.Empty;
            string blockFireFoxYN = "N";

            string url = fireFoxUrl;

            if (url == null)
                return "N";//continue;
            if (url == "")
                return "N";// continue;
            if (url.Contains("https://defender.joygreen.kr/warning2022.asp"))
            {
                string[] result = url.Split(new string[] { $@"?" }, StringSplitOptions.None);
                string tempUrl = string.Empty;
                if (url.Length >= 2)
                {
                    tempUrl = result[1];
                    string[] result2 = tempUrl.Split(new string[] { $@"=" }, StringSplitOptions.None);
                    tempUrl = result2[1];
                }
                Thread.Sleep(1000);
                MovingChrome_http(tempUrl);
                return "N";// continue;
            }

            for (int i = 0; i <= urlListCrNow.Count - 1; i++)//한번 통과된것은 다시 검색하지 않는다.
            {
                if (urlListCrNow[i].Equals(url))
                {
                    checkUrl = 1;
                }
            }

            
            return blockFireFoxYN;
        }//private string blockFireFoxYN()

        private void SearchFireFox()
        {
            try
            {
                AutomationElement root = AutomationElement.RootElement.FindFirst(TreeScope.Children,
                new PropertyCondition(AutomationElement.ClassNameProperty, "MozillaWindowClass"));

                if (root == null)
                {
                    //MessageBox.Show("FireFox를 실행해 주세요.");
                    return;
                }

                IntPtr WindowHandle = new IntPtr(root.Current.NativeWindowHandle);

                Condition toolBar = new AndCondition(
                new PropertyCondition(AutomationElement.ControlTypeProperty, ControlType.ToolBar),

                //영문판 파이어폭스
                new PropertyCondition(AutomationElement.NameProperty, "Browser tabs")
                );
                var tool = root.FindFirst(TreeScope.Children, toolBar);

                if (tool == null)
                {
                    toolBar = new AndCondition(
                    new PropertyCondition(AutomationElement.ControlTypeProperty, ControlType.ToolBar),
                        new PropertyCondition(AutomationElement.NameProperty, "브라우저 탭")
                    );
                    tool = root.FindFirst(TreeScope.Children, toolBar);
                }


                var tool2 = TreeWalker.ControlViewWalker.GetNextSibling(tool);

                var children = tool2.FindAll(TreeScope.Children, Condition.TrueCondition);

                foreach (AutomationElement item in children)
                {
                    foreach (AutomationElement i in item.FindAll(TreeScope.Children, Condition.TrueCondition))
                    {
                        foreach (AutomationElement ii in i.FindAll(TreeScope.Element, Condition.TrueCondition))
                        {
                            //영문판인경우 edit
                            if (ii.Current.LocalizedControlType == "편집" || ii.Current.LocalizedControlType == "edit")
                            {
                                if (!ii.Current.BoundingRectangle.X.ToString().Contains("empty"))
                                {
                                    ValuePattern activeTab = ii.GetCurrentPattern(ValuePattern.Pattern) as ValuePattern;

                                    //현재탭의 URL
                                    var activeUrl = activeTab.Current.Value;
                                    fireFoxUrl = activeUrl;

                                    if (blockFireFoxYN() == "Y")
                                    {
                                        //url 이동하기                                        
                                        ((ValuePattern)activeTab).SetValue(fireFoxUrl);
                                        PostMessage(WindowHandle, WM_KEYDOWN, new IntPtr(VK_RETURN), new IntPtr(0));
                                        PostMessage(WindowHandle, WM_KEYUP, new IntPtr(VK_RETURN), new IntPtr(0));
                                    }
                                    break;

                                }
                            }
                        }
                    }
                }
            }
            catch
            {

            }
        }//private void SearchFireFox()




    }//public partial class Form1 : Form
}//namespace JoyGreen_PF
