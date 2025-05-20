using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace meteo
{
    public partial class Form1 : Form
    {
        static Bitmap canvas = new Bitmap(480, 320);
        Graphics gg = Graphics.FromImage(canvas);
        int PW, PH;
        Point Cpos;                         // カーソル座標
        int[] enX = new int[10];            // 隕石の座標
        int[] enY = new int[10];
        Random rand = new Random();
        int RR;                             // 隕石の半径
        Boolean hitFlg;                     // true:当たった
        int ecnt, ex, ey;                   // 爆発演出
        long msgcnt;                        // メッセージ用カウンタ
        Boolean titleFlg;                   // true:タイトル表示中
        long score;                         // スコア
        Font myFont = new Font("Arial", 16);

        int beamX, beamY;         // ビームの位置
        bool beamActive = false;  // ビームがアクティブかどうか
        int beamCoolTime = 0;     // クールタイムカウンタ（一定時間後に発射可能）
        int beamInterval = 20;   // クールタイム（例：100フレーム後に発射可能）


        public Form1()
        {
            InitializeComponent();
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            pMeteor.Hide();
            pPlayer.Hide();
            pBG.Hide();
            pExp.Hide();
            pGameover.Hide();
            pMsg.Hide();
            pTitle.Hide();
            pBeam.Hide();

            initGame(); // 初期処理
        }

        private void initGame()
        {
            PW = 41; // 自機の幅
            PH = 51; // 自機の高さ
            RR = 70 / 2; // 隕石の半径

            beamActive = false;
            beamCoolTime = 0;


            for (int i = 0; i < 10; i++)
            {
                enX[i] = rand.Next(1, 450);      // 隕石の初期配置座標
                enY[i] = rand.Next(1, 900) - 1000;
            }

            hitFlg = false;  // false:当たっていない
            ecnt = 0; ex = 0; ey = 0; // 爆発の初めの処理で位置を変更する
            msgcnt = 0;
            titleFlg = true; // true:タイトル表示中
            score = 0;
        }

        private void pBase_Click(object sender, EventArgs e)
        {
            if (titleFlg) // タイトル表示中のみ
            {
                if (msgcnt > 20)
                {
                    msgcnt = 0;
                    titleFlg = false;
                }
                return; // タイトル表示中はこの先の処理をしない
            }

            if (msgcnt > 80)
            {
                initGame();
            }

            // ここにビーム発射処理を追加
            if (!beamActive && beamCoolTime >= beamInterval)
            {
                beamX = Cpos.X + (PW / 2) - 2;  // 自機の中心から発射
                beamY = 220;                    // 自機の高さからスタート
                beamActive = true;
                beamCoolTime = 0;
            }

        }



        // 爆発演出
        private void playerExplosion()
        {
            ecnt += 4;
            if (ecnt > 40)
            {
                ecnt = 8;
                ex = Cpos.X + rand.Next(40); // 爆発の位置を変更
                ey = 220 + rand.Next(50);
            }

            // 爆発演出中の描画は、すべてここで行う
            gg.DrawImage(pBG.Image, new Rectangle(0, 0, 480, 320));
            for (int i = 0; i < 10; i++)
            {
                gg.DrawImage(pMeteor.Image,
                    new Rectangle(enX[i], enY[i], RR * 2, RR * 2));
            }
            gg.DrawImage(pPlayer.Image, new Rectangle(Cpos.X, 220, PW, PH));
            gg.DrawImage(pExp.Image, new Rectangle(ex - ecnt / 2, ey - ecnt / 2, ecnt, ecnt));
            msgcnt++;
            if (msgcnt > 60)
            {
                gg.DrawImage(pGameover.Image, new Rectangle(70, 80, 350, 60));
                if (msgcnt % 80 > 20)
                {
                    gg.DrawImage(pMsg.Image, new Rectangle(110, 190, 271, 26));
                }
            }
            gg.DrawString("SCORE: " + score.ToString(),
                myFont, Brushes.White, 10, 10);
            pBase.Image = canvas;
        }

        // タイトル表示
        private void dispTitle()
        {
            msgcnt++;
            // タイトル表示中の描画は、すべてここで行う
            gg.DrawImage(pBG.Image, new Rectangle(0, 0, 480, 320));
            gg.DrawImage(pTitle.Image, new Rectangle(70, 80, 350, 60));
            if (msgcnt % 60 > 20)
            {
                gg.DrawImage(pMsg.Image, new Rectangle(110, 190, 271, 26));
            }
            pBase.Image = canvas;
        }

        private void timer1_Tick(object sender, EventArgs e)
        {
            if (titleFlg)
            {
                dispTitle();
                return; // タイトル表示中はこの先の処理をしない
            }

            if (hitFlg)
            {
                playerExplosion(); // 自機と隕石が当たったときの処理
                return;
            }



            gg.DrawImage(pBG.Image, new Rectangle(0, 0, 480, 320));

            // 隕石の移動
            for (int i = 0; i < 10; i++)
            {
                enY[i] += 5;
                gg.DrawImage(pMeteor.Image,
                    new Rectangle(enX[i], enY[i], RR * 2, RR * 2));
                if (enY[i] > pBase.Height) // 画面外へ出たら上に戻す
                {
                    enX[i] = rand.Next(1, 450);
                    enY[i] = rand.Next(1, 300) - 400;
                }
            }

            // ビーム処理
            if (beamActive)
            {
                beamY -= 15;
                gg.DrawImage(pBeam.Image, new Rectangle(beamX, beamY, 20, 40));

                // ビームと隕石の当たり判定
                for (int i = 0; i < 10; i++)
                {
                    int ecx = enX[i] + RR;
                    int ecy = enY[i] + RR;
                    int dis = (ecx - (beamX + 2)) * (ecx - (beamX + 2)) + (ecy - (beamY + 10)) * (ecy - (beamY + 10));
                    if (dis < RR * RR)
                    {
                        // 隕石を再配置（破壊演出に変えてもOK）
                        enX[i] = rand.Next(1, 450);
                        enY[i] = rand.Next(1, 300) - 400;

                        beamActive = false; // ビーム消滅
                        break;
                    }
                }

                // ビームが画面外へ
                if (beamY < 0)
                {
                    beamActive = false;
                }
            }
            else
            {
                beamCoolTime++;
            }

            Cpos = PointToClient(Cursor.Position);
            if (Cpos.X < 0)
            {
                Cpos.X = 0;
            }
            if (Cpos.X > Width - PW)
            {
                Cpos.X = Width - PW;
            }

            gg.DrawImage(pPlayer.Image, new Rectangle(Cpos.X, 220, PW, PH));

            score++;
            gg.DrawString("SCORE: " + score.ToString(),
                myFont, Brushes.White, 10, 10);
            pBase.Image = canvas;

            hitCheck(); // 当たり判定
        }

        // 自機と隕石の当たり判定
        private void hitCheck()
        {
            int pcx = Cpos.X + (PW / 2); // 自機の中心座標
            int pcy = 220 + (PH / 2);
            int ecx, ecy, dis; // 自機と隕石の距離計算用

            for (int i = 0; i < 10; i++)
            {
                ecx = enX[i] + RR;
                ecy = enY[i] + RR;
                dis = (ecx - pcx) * (ecx - pcx) + (ecy - pcy) * (ecy - pcy);
                if (dis < RR * RR)
                {
                    hitFlg = true; // true:当たった
                    break;         // for から抜ける
                }
            }
        }
    }
}

