using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

using System.Threading;

namespace WindowsFormsApp4_multiThread
{
    public partial class Form1 : Form
    {
        static BackgroundWorker worker;
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
    }
}
