using System;
using System.ComponentModel;
using System.Threading;
using System.Windows.Forms;

namespace WindowsFormsApp4_multiThread
{

    //스레드
    //대표 메서드
    //Start(), Sleep(), Abort(), Join()

    public partial class Form1 : Form
    {
        static BackgroundWorker worker;
        static int sharedata = 0;
        static object lockObject = new object();

        public Form1()
        {
            InitializeComponent();

            worker = new BackgroundWorker();

            worker.WorkerReportsProgress = true;    //ReportsProgress() 사용가능하게 설정
            //ReportsProgress
            //작업 중간중간 진행률을 보고할때 사용하는 메서드
            //진행률: 보톤 0~100 사이의 정수 -> %로 표현

            worker.WorkerSupportsCancellation = false;  //취소 기능 x

            //이벤트 연결 +=로 등록
            worker.DoWork += Worker_DoWork; //작업 내용 작성할 메서드 연결
            worker.ProgressChanged += Worker_ProgressChanged;
            worker.RunWorkerCompleted += Worker_Completed;

            //스레드
            Thread t1 = new Thread(update1);
            Thread t2 = new Thread(update2);

            t1.Start();
            t2.Start();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            if (!worker.IsBusy)
            {
                label1.Text = "작업 시작";
                progressBar1.Value = 0;     //프로그래스 바 초기 설정
                worker.RunWorkerAsync();    //비동기 작업 실행
            }
        }

        //백그라운드 스레드에서 실행
        private void Worker_DoWork(object send, DoWorkEventArgs e)
        {
            for (int i = 0; i <= 100; i++)
            {
                Thread.Sleep(50);   //0.05초 지연
                worker.ReportProgress(i);   //위에서 false로 하면 동작 안하겠지

                //현재 진행 상황 i값을 ui 스레드로 전달
            }
        }

        //진행률 ui 갱신
        private void Worker_ProgressChanged(object send, ProgressChangedEventArgs e)
        {
            progressBar1.Value = e.ProgressPercentage;  // ReportsProgress에서 전달한 값

            label1.Text = $"{e.ProgressPercentage} 진행 중 ...";


        }

        private void Worker_Completed(object send, RunWorkerCompletedEventArgs e)
        {
            label1.Text = "작업 완료";

        }

        //스레드
        private void update1()
        {
            lock (lockObject)
            {
                for (int i = 0; i < 10; i++)
                {
                    sharedata++;
                    Thread.Sleep(10);

                    //ui 스레드 분기 처리
                    if (textBox1.InvokeRequired)    //->bool 타입 -> true면 지금 코드가 ui스레드 아닌 다른 스레드에서 실행중이라는 뜻
                    {
                        textBox1.Invoke(new MethodInvoker(() =>
                        {
                            textBox1.Text += $"1: {sharedata}\r\n";
                        }));
                    }
                    else
                    {
                        textBox1.Text += $"1: {sharedata}\r\n";
                    }


                    //textBox1.Text += $"1: {sharedata}\r\n";
                }
            }
            
        }
        private void update2()
        {
            lock (lockObject)
            {
                for (int i = 0; i < 10; i++)
                {
                    sharedata++;
                    Thread.Sleep(10);

                    if (textBox1.InvokeRequired)    //->bool 타입 -> true면 지금 코드가 ui스레드 아닌 다른 스레드에서 실행중이라는 뜻
                    {
                        textBox1.Invoke(new MethodInvoker(() =>
                        {
                            textBox1.Text += $"2: {sharedata}\r\n";
                        }));
                    }
                    else
                    {
                        textBox1.Text += $"2: {sharedata}\r\n";
                    }

                }
            }
        }
    }
}
