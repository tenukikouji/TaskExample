using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Windows.Threading;

namespace TaskExample
{
    /// <summary>
    /// MainWindow.xaml の相互作用ロジック
    /// </summary>
    public partial class MainWindow : Window
    {
        private int countSec = 0;

        private DispatcherTimer timer = new DispatcherTimer();

        public MainWindow()
        {
            InitializeComponent();

            timer.Interval = new TimeSpan(0, 0, 0, 0, 100);
            timer.Tick += (s, e) =>
            {
                countSec++;
                this.time.Text = $"{countSec / 10}.{countSec % 10}sec";
            };
        }

        private void SetOutputText(string output1, string output2)
        {
            this.output1.Text = output1;
            this.output2.Text = output2;
        }

        private void ExecuteBefore()
        {
            this.SetOutputText(string.Empty, string.Empty);
            this.status.Text = "実行中";
            this.countSec = 0;
            this.time.Text = "0.0sec";
            this.timer.Start();
        }

        private void ExecuteAfter()
        {
            this.status.Text = "完了";
            timer.Stop();
        }

        /// <summary>
        /// 入力値を二乗に変換して、5秒後に応答する。
        /// </summary>
        /// <param name="input">入力値</param>
        /// <returns>実行中タスク</returns>
        private Task<string> ExecuteTask(string input)
        {
            Task<string> task = Task.Run(() =>
            {
                Thread.Sleep(5000);
                if (int.TryParse(input, out int output))
                {
                    return Math.Pow(output, 2).ToString();
                }

                return $"{input}はintに変換出来ません";
            });

            return task;
        }

        /// <summary>
        /// 入力値を二乗に変換して、5秒後に応答する。
        /// </summary>
        /// <param name="input">入力値</param>
        /// <returns>非実行中タスク</returns>
        private Task<string> NoExecuteTask(string input)
        {
            Task<string> task = new Task<string>(() =>
            {
                Thread.Sleep(5000);
                if (int.TryParse(input, out int output))
                {
                    return Math.Pow(output, 2).ToString();
                }

                return $"{input}はintに変換出来ません";
            });

            return task;
        }

        /// <summary>
        /// 入力欄の値を二乗に変換した値を出力欄に記載し、voidを応答する。
        /// </summary>
        private async void ExecuteReturnVoid()
        {
            string result1 = await this.ExecuteTask(Input1.Text);
            this.output1.Text = result1;

            string result2 = await this.ExecuteTask(Input2.Text);
            this.output2.Text = result2;
        }

        /// <summary>
        /// 入力欄の値を二乗に変換した値を出力欄に記載し、Taskを応答する。
        /// </summary>
        /// <returns>実行中タスク</returns>
        private async Task ExecuteReturnTaskAsync()
        {
            string result1 = await this.ExecuteTask(Input1.Text);
            this.output1.Text = result1;

            string result2 = await this.ExecuteTask(Input2.Text);
            this.output2.Text = result2;
        }

        private async void Button1_Click(object sender, RoutedEventArgs e)
        {
            this.ExecuteBefore();

            this.output1.Text = await this.ExecuteTask(Input1.Text);
            this.output2.Text = await this.ExecuteTask(Input2.Text);

            this.ExecuteAfter();
        }

        private async void Button2_Click(object sender, RoutedEventArgs e)
        {
            this.ExecuteBefore();

            this.SetOutputText(await this.ExecuteTask(Input1.Text), await this.ExecuteTask(Input2.Text));

            this.ExecuteAfter();
        }


        private async void Button3_Click(object sender, RoutedEventArgs e)
        {
            this.ExecuteBefore();

            Task<string> output1Task = this.ExecuteTask(Input1.Text);
            Task<string> output2Task = this.ExecuteTask(Input2.Text);

            await Task.WhenAll(output1Task, output2Task);

            this.output1.Text = output1Task.Result;
            this.output2.Text = output2Task.Result;

            this.ExecuteAfter();
        }

        private async void Button4_Click(object sender, RoutedEventArgs e)
        {
            this.ExecuteBefore();

            string result1 = await this.NoExecuteTask(Input1.Text);
            // Taskが非実行中である為、↑のawaitで永遠に待機する (awaitを使っている為、GUIは固まらない)
            string result2 = await this.NoExecuteTask(Input2.Text);

            this.output1.Text = result1;
            this.output2.Text = result2;

            this.ExecuteAfter();
        }

        private async void Button5_Click(object sender, RoutedEventArgs e)
        {
            this.ExecuteBefore();

            Task<string> output1Task = this.NoExecuteTask(Input1.Text);
            Task<string> output2Task = this.NoExecuteTask(Input2.Text);

            output1Task.Start();
            output2Task.Start();

            await Task.WhenAll(output1Task, output2Task);

            this.output1.Text = output1Task.Result;
            this.output2.Text = output2Task.Result;

            this.ExecuteAfter();
        }

        private void Button6_Click(object sender, RoutedEventArgs e)
        {
            this.ExecuteBefore();

            // 下記に置き換えるとビルドエラーになる
            // await this.ExecuteReturnVoid();
            this.ExecuteReturnVoid();

            this.ExecuteAfter();
        }

        private async void Button7_Click(object sender, RoutedEventArgs e)
        {
            this.ExecuteBefore();

            await this.ExecuteReturnTaskAsync();

            this.ExecuteAfter();
        }
    }
}
