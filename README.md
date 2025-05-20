✅ 実装の流れ（まとめ）
クリックイベントでビーム発射（pBase_Click() 内で）

ビームが画面を上昇し、隕石と当たると破壊

一度に1本だけ発射できる（※連射制限）

🔧 1. 変数の追加（クラスの先頭付近）
csharp
コードをコピーする
int beamX, beamY;         // ビームの位置
bool beamActive = false;  // ビームが存在しているか
int beamCoolTime = 0;     // クールタイム用カウンタ
int beamInterval = 20;    // クールタイム制限（20フレームなど）
🧩 2. pBase_Click() 内の修正（クリック時に発射）
csharp
コードをコピーする
private void pBase_Click(object sender, EventArgs e)
{
    if (titleFlg)
    {
        if (msgcnt > 20)
        {
            msgcnt = 0;
            titleFlg = false;
        }
        return;
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
🧩 3. timer1_Tick() にビーム処理を追加
// 隕石の移動の後か、score++; の前などに追記します。

csharp
コードをコピーする
// ビーム処理
if (beamActive)
{
    beamY -= 15;
    gg.FillRectangle(Brushes.Cyan, beamX, beamY, 5, 20); // ビームの描画

    // ビームと隕石の当たり判定
    for (int i = 0; i < 10; i++)
    {
        int ecx = enX[i] + RR;
        int ecy = enY[i] + RR;
        int dis = (ecx - (beamX + 2)) * (ecx - (beamX + 2)) + (ecy - (beamY + 10)) * (ecy - (beamY + 10));
        if (dis < RR * RR)
        {
            // 隕石を破壊（再配置）
            enX[i] = rand.Next(1, 450);
            enY[i] = rand.Next(1, 300) - 400;
            beamActive = false;
            break;
        }
    }

    // 画面外でビーム消滅
    if (beamY < 0)
    {
        beamActive = false;
    }
}
else
{
    beamCoolTime++;
}
🧩 4. initGame() にリセット処理を追加
csharp
コードをコピーする
beamActive = false;
beamCoolTime = 0;
✅ これでできること
クリックで1発ずつビームを発射

隕石に当たれば破壊（再出現）

画面外に出たら消える

一定時間（クールタイム）経過しないと連射できない

🎨 オプション
gg.FillRectangle(...) → 画像を使いたいなら pBeam.Image を DrawImage() で描画可能。

エフェクトや音追加も可能です。

必要があれば以下のような強化もできます：

ビームの数制限・複数発射

隕石が破壊されたときスコア追加

ビームにエフェクトをつける

