using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Net;
using System.IO;
using System.Xml;
using System.Net.Mail;

namespace HTTPClientExample
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }
        delegate void UpdateGUI(string s);
        void UpdateTextBox(string s)
        {
            textBox1.Text+= s + "\r\n";
        }
        void OutputText(string s)
        {
            textBox1.Invoke(new UpdateGUI(UpdateTextBox), new object[] { s});
        }

        private void button1_Click(object sender, EventArgs e)
        {
            work();
            timer1.Start();
            button1.Enabled = false;
        }
        public void work()
        {
            string change = "";
            string sendd = ""; 
            
            string city = "";
           // string city1 = "";
            double buyy;
            double selll;
            double[] buy = new double[20];
            double[] sell = new double[20];
            for (int i = 1; i < 20; i++)
            {
                buy[i] = -1;
                sell[i] = -1;  
            }

            double[] buytest = new double[20];
            double[] selltest = new double[20];
            for (int i = 1; i < 20; i++)
            {
                buytest[i] = -1;
                selltest[i] = -1;
            }

            //int count = 0;
            String link = "http://www.sjc.com.vn/xml/tygiavang.xml";
            HttpWebRequest request = WebRequest.CreateHttp(link);
            HttpWebResponse response = (HttpWebResponse)request.GetResponse();
            StreamReader sr = new StreamReader(response.GetResponseStream(), Encoding.UTF8);
            string content = sr.ReadToEnd();
            sr.Close();
           
            XmlReader reader = XmlReader.Create(new StringReader(content));
            reader.ReadToFollowing("ratelist");
            reader.MoveToFirstAttribute(); //thuoc tinh "updated"
            System.Console.WriteLine("Cap nhat luc: " + reader.Value);
            OutputText("Cap nhat luc: " + reader.Value);
            reader.MoveToNextAttribute(); // Thuoc tinh unit
            System.Console.WriteLine("Don vi tinh: " + reader.Value);
            OutputText("Don vi tinh: " + reader.Value);
            while (reader.ReadToFollowing("city")) //duyet cac Element city
            {
                reader.MoveToFirstAttribute();
                System.Console.WriteLine("Thanh pho: " + reader.Value);
                city = reader.Value;
                OutputText("Thanh pho: " + reader.Value);
               
                //  while (r.ReadToFollowing("item")) { count++; }
                //while (r.ReadToFollowing("item"))
                if (city == "Hồ Chí Minh")
                {
                    reader.MoveToElement();
                    XmlReader r = reader.ReadSubtree();
                    for (int i = 1; i <= 8; i++)
                    {
                        r.ReadToFollowing("item");
                        r.MoveToFirstAttribute();//buy
                        System.Console.WriteLine("Gia mua: " + reader.Value);
                        buy[i] = Convert.ToDouble(reader.Value);
                        if (buy[i] >= 47.750) { change += "Gia mua: " + Convert.ToString(buy[i]) + "/";
                            //  MessageBox.Show(Convert.ToString( buy[i]));
                            buytest[i] = buy[i];
                        }
                        OutputText("Gia mua: " + reader.Value);
                        r.MoveToNextAttribute(); //sell
                        System.Console.WriteLine("Gia ban: " + reader.Value);
                        sell[i] = Convert.ToDouble(reader.Value);
                        if (sell[i] >= 48.635) { change += "Gia ban: " + Convert.ToString(sell[i]) + "/";
                            selltest[i] = selltest[i];
                        }
                        OutputText("Gia ban: " + reader.Value);
                        r.MoveToNextAttribute(); //type
                        System.Console.WriteLine("Loai: " + reader.Value);
                        if (buytest[i] != (-1) || selltest[i] != (-1))
                        { change += ("loai: " + reader.Value + "\n\t"); }
                        OutputText("Loai: " + reader.Value);
                    }
                }
                if(city != "Hồ Chí Minh")
                {
                    reader.MoveToElement();
                    XmlReader r = reader.ReadSubtree();
                    while (r.ReadToFollowing("item"))
                    {
                       // r.ReadToFollowing("item");
                        r.MoveToFirstAttribute();//buy
                        System.Console.WriteLine("Gia mua: " + reader.Value);
                        buyy = Convert.ToDouble(reader.Value);
                        if (buyy >= 47.750) { change += "Gia mua: " + Convert.ToString(buyy) + "/"; }
                        OutputText("Gia mua: " + reader.Value);
                        r.MoveToNextAttribute(); //sell
                        System.Console.WriteLine("Gia ban: " + reader.Value);
                        selll = Convert.ToDouble(reader.Value);
                        if (selll >= 48.635) { change += "Gia ban: " + Convert.ToString(selll) + "/"; }
                        OutputText("Gia ban: " + reader.Value);
                        r.MoveToNextAttribute(); //type
                        System.Console.WriteLine("Loai: " + reader.Value);
                        if (change!="") { change += "loai: " + reader.Value + "/"; }
                        OutputText("Loai: " + reader.Value);
                    }
                }
                if (change != "") {
                    change +=" cua tp: "+ city+"\n\r";
                    sendd += change;
                    change = "";
                }
            }
            System.Console.WriteLine(sendd);
            string sent = sendd.Replace("\n\r","<br/>");
            string hcmSent = sent.Replace("\n\t", "<br/>");
            if (sendd != "") { MessageBox.Show(sendd); maill(hcmSent); }
        }
        public void maill(string e)
        {
           
            using (MailMessage mail = new MailMessage())
            {
                mail.From = new MailAddress(" *EnterEmailSend* vd:(abc@gmail.com) ");
                mail.To.Add("EnterEmailRecieve");
                mail.Subject = "Cảnh báo giá vàng";
                mail.Body = e;
                mail.IsBodyHtml = true;
               // mail.Attachments.Add(new Attachment("C:\\file.zip"));

                using (SmtpClient smtp = new SmtpClient("smtp.gmail.com", 587))
                {
                    smtp.Credentials = new NetworkCredential("*EnterEmailSend*", "*EnterPasswordEmailSend*");
                    smtp.EnableSsl = true;
                    smtp.Send(mail);
                }
            }

        }
        private void timer1_Tick(object sender, EventArgs e)
        {
            work();
        }
    }
}
