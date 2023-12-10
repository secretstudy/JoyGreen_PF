namespace JoyGreen_PF
{
    partial class Form1
    {
        /// <summary>
        /// 필수 디자이너 변수입니다.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// 사용 중인 모든 리소스를 정리합니다.
        /// </summary>
        /// <param name="disposing">관리되는 리소스를 삭제해야 하면 true이고, 그렇지 않으면 false입니다.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form 디자이너에서 생성한 코드

        /// <summary>
        /// 디자이너 지원에 필요한 메서드입니다. 
        /// 이 메서드의 내용을 코드 편집기로 수정하지 마세요.
        /// </summary>
        private void InitializeComponent()
        {
            this.components = new System.ComponentModel.Container();
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(Form1));
            this.Trayicon = new System.Windows.Forms.NotifyIcon(this.components);
            this.loginComNo = new System.Windows.Forms.TextBox();
            this.label6 = new System.Windows.Forms.Label();
            this.loginPwd = new System.Windows.Forms.TextBox();
            this.label1 = new System.Windows.Forms.Label();
            this.label8 = new System.Windows.Forms.Label();
            this.label7 = new System.Windows.Forms.Label();
            this.login_btn = new System.Windows.Forms.Button();
            this.loginID = new System.Windows.Forms.TextBox();
            this.label5 = new System.Windows.Forms.Label();
            this.label4 = new System.Windows.Forms.Label();
            this.label3 = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.label9 = new System.Windows.Forms.Label();
            this.joinlabel = new System.Windows.Forms.Label();
            this.read_pw = new System.Windows.Forms.CheckBox();
            this.Context_TrayIcon = new System.Windows.Forms.ContextMenuStrip(this.components);
            this.SuspendLayout();
            // 
            // Trayicon
            // 
            this.Trayicon.Icon = ((System.Drawing.Icon)(resources.GetObject("Trayicon.Icon")));
            this.Trayicon.Visible = true;
            this.Trayicon.MouseClick += new System.Windows.Forms.MouseEventHandler(this.Trayicon_MouseClick);
            // 
            // loginComNo
            // 
            this.loginComNo.Font = new System.Drawing.Font("굴림", 16F);
            this.loginComNo.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(192)))), ((int)(((byte)(193)))), ((int)(((byte)(194)))));
            this.loginComNo.Location = new System.Drawing.Point(473, 475);
            this.loginComNo.Margin = new System.Windows.Forms.Padding(0);
            this.loginComNo.MaxLength = 20;
            this.loginComNo.Name = "loginComNo";
            this.loginComNo.Size = new System.Drawing.Size(262, 56);
            this.loginComNo.TabIndex = 40;
            this.loginComNo.Text = "좌석번호 입력";
            this.loginComNo.MouseDown += new System.Windows.Forms.MouseEventHandler(this.loginComNo_MouseDown);
            // 
            // label6
            // 
            this.label6.AutoSize = true;
            this.label6.Font = new System.Drawing.Font("굴림", 18F);
            this.label6.Location = new System.Drawing.Point(216, 481);
            this.label6.Margin = new System.Windows.Forms.Padding(6, 0, 6, 0);
            this.label6.Name = "label6";
            this.label6.Size = new System.Drawing.Size(212, 48);
            this.label6.TabIndex = 50;
            this.label6.Text = "좌석번호";
            // 
            // loginPwd
            // 
            this.loginPwd.Font = new System.Drawing.Font("굴림", 16F);
            this.loginPwd.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(192)))), ((int)(((byte)(193)))), ((int)(((byte)(194)))));
            this.loginPwd.Location = new System.Drawing.Point(473, 379);
            this.loginPwd.Margin = new System.Windows.Forms.Padding(0);
            this.loginPwd.MaxLength = 20;
            this.loginPwd.Name = "loginPwd";
            this.loginPwd.PasswordChar = '*';
            this.loginPwd.Size = new System.Drawing.Size(622, 56);
            this.loginPwd.TabIndex = 39;
            this.loginPwd.Text = "비밀번호를 입력해주세요";
            this.loginPwd.MouseDown += new System.Windows.Forms.MouseEventHandler(this.loginPwd_MouseDown);
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Font = new System.Drawing.Font("굴림", 18F);
            this.label1.Location = new System.Drawing.Point(216, 395);
            this.label1.Margin = new System.Windows.Forms.Padding(6, 0, 6, 0);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(212, 48);
            this.label1.TabIndex = 49;
            this.label1.Text = "비밀번호";
            // 
            // label8
            // 
            this.label8.AutoSize = true;
            this.label8.Location = new System.Drawing.Point(110, 645);
            this.label8.Margin = new System.Windows.Forms.Padding(6, 0, 6, 0);
            this.label8.Name = "label8";
            this.label8.Size = new System.Drawing.Size(250, 24);
            this.label8.TabIndex = 48;
            this.label8.Text = "     로그인을 해주세요";
            // 
            // label7
            // 
            this.label7.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
            this.label7.Location = new System.Drawing.Point(114, 675);
            this.label7.Margin = new System.Windows.Forms.Padding(6, 0, 6, 0);
            this.label7.Name = "label7";
            this.label7.Size = new System.Drawing.Size(1005, 20);
            this.label7.TabIndex = 47;
            // 
            // login_btn
            // 
            this.login_btn.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(50)))), ((int)(((byte)(161)))), ((int)(((byte)(255)))));
            this.login_btn.Font = new System.Drawing.Font("굴림", 15.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(129)));
            this.login_btn.ForeColor = System.Drawing.Color.White;
            this.login_btn.Location = new System.Drawing.Point(785, 475);
            this.login_btn.Margin = new System.Windows.Forms.Padding(6);
            this.login_btn.Name = "login_btn";
            this.login_btn.Size = new System.Drawing.Size(314, 64);
            this.login_btn.TabIndex = 41;
            this.login_btn.Text = "로그인";
            this.login_btn.UseVisualStyleBackColor = false;
            this.login_btn.Click += new System.EventHandler(this.login_btn_Click);
            // 
            // loginID
            // 
            this.loginID.Font = new System.Drawing.Font("굴림", 16F);
            this.loginID.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(192)))), ((int)(((byte)(193)))), ((int)(((byte)(194)))));
            this.loginID.Location = new System.Drawing.Point(473, 289);
            this.loginID.Margin = new System.Windows.Forms.Padding(0);
            this.loginID.MaxLength = 20;
            this.loginID.Name = "loginID";
            this.loginID.Size = new System.Drawing.Size(622, 56);
            this.loginID.TabIndex = 38;
            this.loginID.Text = "아이디를 입력해주세요";
            this.loginID.MouseDown += new System.Windows.Forms.MouseEventHandler(this.loginID_MouseDown);
            // 
            // label5
            // 
            this.label5.AutoSize = true;
            this.label5.Font = new System.Drawing.Font("굴림", 18F);
            this.label5.Location = new System.Drawing.Point(216, 305);
            this.label5.Margin = new System.Windows.Forms.Padding(6, 0, 6, 0);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(164, 48);
            this.label5.TabIndex = 45;
            this.label5.Text = "아이디";
            // 
            // label4
            // 
            this.label4.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
            this.label4.Location = new System.Drawing.Point(114, 199);
            this.label4.Margin = new System.Windows.Forms.Padding(6, 0, 6, 0);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(984, 20);
            this.label4.TabIndex = 44;
            // 
            // label3
            // 
            this.label3.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
            this.label3.Location = new System.Drawing.Point(114, 183);
            this.label3.Margin = new System.Windows.Forms.Padding(6, 0, 6, 0);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(984, 20);
            this.label3.TabIndex = 43;
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Font = new System.Drawing.Font("굴림", 17F);
            this.label2.Location = new System.Drawing.Point(994, 129);
            this.label2.Margin = new System.Windows.Forms.Padding(6, 0, 6, 0);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(112, 46);
            this.label2.TabIndex = 46;
            this.label2.Text = "닫기";
            this.label2.Click += new System.EventHandler(this.label2_Click);
            // 
            // label9
            // 
            this.label9.AutoSize = true;
            this.label9.Font = new System.Drawing.Font("굴림", 30F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Pixel);
            this.label9.Location = new System.Drawing.Point(107, 129);
            this.label9.Margin = new System.Windows.Forms.Padding(0);
            this.label9.Name = "label9";
            this.label9.Size = new System.Drawing.Size(106, 30);
            this.label9.TabIndex = 42;
            this.label9.Text = "로그인";
            // 
            // joinlabel
            // 
            this.joinlabel.AutoSize = true;
            this.joinlabel.Font = new System.Drawing.Font("굴림", 24F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Pixel);
            this.joinlabel.Location = new System.Drawing.Point(985, 40);
            this.joinlabel.Margin = new System.Windows.Forms.Padding(0);
            this.joinlabel.Name = "joinlabel";
            this.joinlabel.Size = new System.Drawing.Size(110, 24);
            this.joinlabel.TabIndex = 51;
            this.joinlabel.Text = "회원가입";
            this.joinlabel.Click += new System.EventHandler(this.joinlabel_Click);
            // 
            // read_pw
            // 
            this.read_pw.AutoSize = true;
            this.read_pw.Font = new System.Drawing.Font("굴림", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(129)));
            this.read_pw.Location = new System.Drawing.Point(1127, 391);
            this.read_pw.Margin = new System.Windows.Forms.Padding(4);
            this.read_pw.Name = "read_pw";
            this.read_pw.Size = new System.Drawing.Size(281, 36);
            this.read_pw.TabIndex = 69;
            this.read_pw.Text = "비밀번호 보이기";
            this.read_pw.UseVisualStyleBackColor = true;
            this.read_pw.CheckedChanged += new System.EventHandler(this.read_pw_CheckedChanged);
            // 
            // Context_TrayIcon
            // 
            this.Context_TrayIcon.ImageScalingSize = new System.Drawing.Size(32, 32);
            this.Context_TrayIcon.Name = "contextMenuStrip1";
            this.Context_TrayIcon.Size = new System.Drawing.Size(61, 4);
            // 
            // Form1
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(13F, 24F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(1583, 794);
            this.Controls.Add(this.read_pw);
            this.Controls.Add(this.joinlabel);
            this.Controls.Add(this.loginComNo);
            this.Controls.Add(this.label6);
            this.Controls.Add(this.loginPwd);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.label8);
            this.Controls.Add(this.label7);
            this.Controls.Add(this.login_btn);
            this.Controls.Add(this.loginID);
            this.Controls.Add(this.label5);
            this.Controls.Add(this.label4);
            this.Controls.Add(this.label3);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.label9);
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.Name = "Form1";
            this.Text = "Form1";
            this.Load += new System.EventHandler(this.Form1_Load);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.NotifyIcon Trayicon;
        private System.Windows.Forms.TextBox loginComNo;
        private System.Windows.Forms.Label label6;
        private System.Windows.Forms.TextBox loginPwd;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Label label8;
        private System.Windows.Forms.Label label7;
        private System.Windows.Forms.Button login_btn;
        private System.Windows.Forms.TextBox loginID;
        private System.Windows.Forms.Label label5;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Label label9;
        private System.Windows.Forms.Label joinlabel;
        private System.Windows.Forms.CheckBox read_pw;
        private System.Windows.Forms.ContextMenuStrip Context_TrayIcon;
    }
}

