using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Data.SqlClient;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Xml;

namespace JoyGreen_PF
{
    public partial class Join : Form
    {
        public static string DB_SV = "";
        public static string cliVersion = "";
        public static string DB_Name = "";
        public static SqlConnection dbConn; 

        public Join()
        {
            InitializeComponent();
            this.TopMost = true;// 창 안내려 가게 고정하기
        }

        private void loginlabel_Click(object sender, EventArgs e)
        {
            this.Hide();

            Form1 form1 = new Form1();
            form1.Show();
        }

        private void join_btn_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrWhiteSpace(joinName.Text) || string.IsNullOrWhiteSpace(joinID.Text) || string.IsNullOrWhiteSpace(joinPwd.Text))
            {
                MessageBox.Show("모든 필드를 입력하세요.");
                return;
            }

            try
            {
                // XML에서 DB 연결 정보 가져오기
                XmlDocument xml = new XmlDocument();
                string url = Application.StartupPath + @"\Xml\DB_Xml.xml";
                xml.Load(url);

                XmlElement KeyList = xml.DocumentElement;
                XmlNodeList xmlList = xml.SelectNodes("/CONFIG/DB_INFO");

                foreach (XmlNode node1 in xmlList)
                {
                    DB_SV = node1["DATA_SOURCE"].InnerText;
                    DB_Name = node1["DATABASE"].InnerText;
                }

                string DBCon = $"Server={DB_SV};Database={DB_Name};User Id=joygreen_pf;Password=joygreen;";

                // DB 연결 및 쿼리 실행
                using (SqlConnection dbConn = new SqlConnection(DBCon))
                {
                    dbConn.Open();

                    string insertQuery = "INSERT INTO TBL_JG_STORE (name, id, pw) VALUES (@Name, @ID, @Password)";
                    SqlCommand command = new SqlCommand(insertQuery, dbConn);

                    // 파라미터를 이용하여 SQL Injection 방지 및 안전한 쿼리 실행
                    command.Parameters.AddWithValue("@Name", joinName.Text);
                    command.Parameters.AddWithValue("@ID", joinID.Text);
                    command.Parameters.AddWithValue("@Password", joinPwd.Text);

                    int rowsAffected = command.ExecuteNonQuery();
                    if (rowsAffected == 1)
                    {
                        MessageBox.Show(joinName.Text + " 회원가입 완료");
                        Close();
                    }
                    else
                    {
                        MessageBox.Show("비정상 입력 정보, 재확인 요망");
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        private void read_pw_CheckedChanged(object sender, EventArgs e)
        {
            // 체크시 비밀번호를 보이게 한다
            if (read_pw.Checked == true)
            {
                // default(char)는 기본값으로 설정한다는 기능
                // 기본값은 빈문자열을 나타내며 이경우 평문으로 표시된다.
                // 즉 입력한 비밀번호가 숨겨지지 않고 표시된다.
                joinPwd.PasswordChar = default(char);
            }
            //체크 안하면 *로 표시한다.
            else
                joinPwd.PasswordChar = '*';
        }

        private void label2_Click(object sender, EventArgs e)
        {
            this.Close();
        }
    }
}
