namespace TellerApp.WinForms;

partial class TellerForm
{
    /// <summary>
    ///  Required designer variable.
    /// </summary>
    private System.ComponentModel.IContainer components = null;

    /// <summary>
    ///  Clean up any resources being used.
    /// </summary>
    /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
    protected override void Dispose(bool disposing)
    {
        if (disposing && (components != null))
        {
            components.Dispose();
        }
        base.Dispose(disposing);
    }

    #region Windows Form Designer generated code

    /// <summary>
    ///  Required method for Designer support - do not modify
    ///  the contents of this method with the code editor.
    /// </summary>
    private void InitializeComponent()
    {
        pnlHeader = new Panel();
        lblFormTitle = new Label();
        pnlAccent = new Panel();
        tabMain = new TabControl();
        tabCallNext = new TabPage();
        lblCallStatus = new Label();
        lblCallResult = new Label();
        btnCallNext = new Button();
        btnRefreshNextWaiting = new Button();
        lblNextWaiting = new Label();
        numCounterNumber = new NumericUpDown();
        lblCounterNumber = new Label();
        tabTransfer = new TabPage();
        lblTransferResult = new Label();
        btnTransfer = new Button();
        txtAmount = new TextBox();
        lblAmount = new Label();
        lblToOwner = new Label();
        txtToAccount = new TextBox();
        lblTo = new Label();
        lblFromOwner = new Label();
        txtFromAccount = new TextBox();
        lblFrom = new Label();
        tabRates = new TabPage();
        lblRateStatus = new Label();
        btnUpdateRate = new Button();
        txtSellRate = new TextBox();
        lblSellRate = new Label();
        txtBuyRate = new TextBox();
        lblBuyRate = new Label();
        txtCurrencyCode = new TextBox();
        lblCurrencyCode = new Label();
        btnRefreshRates = new Button();
        lvRates = new ListView();
        chCurrency = new ColumnHeader();
        chBuy = new ColumnHeader();
        chSell = new ColumnHeader();
        chUpdated = new ColumnHeader();
        pnlHeader.SuspendLayout();
        tabMain.SuspendLayout();
        tabCallNext.SuspendLayout();
        ((System.ComponentModel.ISupportInitialize)numCounterNumber).BeginInit();
        tabTransfer.SuspendLayout();
        tabRates.SuspendLayout();
        SuspendLayout();
        //
        // pnlHeader
        //
        pnlHeader.BackColor = BankColors.Navy;
        pnlHeader.Controls.Add(lblFormTitle);
        pnlHeader.Dock = DockStyle.Top;
        pnlHeader.Location = new Point(0, 0);
        pnlHeader.Name = "pnlHeader";
        pnlHeader.Size = new Size(620, 70);
        pnlHeader.TabIndex = 0;
        //
        // lblFormTitle
        //
        lblFormTitle.Dock = DockStyle.Fill;
        lblFormTitle.Font = new Font("Segoe UI", 15F, FontStyle.Bold);
        lblFormTitle.ForeColor = Color.White;
        lblFormTitle.Location = new Point(0, 0);
        lblFormTitle.Name = "lblFormTitle";
        lblFormTitle.Size = new Size(620, 70);
        lblFormTitle.TabIndex = 0;
        lblFormTitle.Text = "Теллер апп";
        lblFormTitle.TextAlign = ContentAlignment.MiddleCenter;
        //
        // pnlAccent
        //
        pnlAccent.BackColor = BankColors.Gold;
        pnlAccent.Dock = DockStyle.Top;
        pnlAccent.Location = new Point(0, 70);
        pnlAccent.Name = "pnlAccent";
        pnlAccent.Size = new Size(620, 4);
        pnlAccent.TabIndex = 1;
        //
        // tabMain
        //
        tabMain.Controls.Add(tabCallNext);
        tabMain.Controls.Add(tabTransfer);
        tabMain.Controls.Add(tabRates);
        tabMain.Location = new Point(10, 84);
        tabMain.Name = "tabMain";
        tabMain.SelectedIndex = 0;
        tabMain.Size = new Size(600, 460);
        tabMain.TabIndex = 2;
        //
        // tabCallNext
        //
        tabCallNext.BackColor = BankColors.Background;
        tabCallNext.Controls.Add(lblCallStatus);
        tabCallNext.Controls.Add(lblCallResult);
        tabCallNext.Controls.Add(btnCallNext);
        tabCallNext.Controls.Add(btnRefreshNextWaiting);
        tabCallNext.Controls.Add(lblNextWaiting);
        tabCallNext.Controls.Add(numCounterNumber);
        tabCallNext.Controls.Add(lblCounterNumber);
        tabCallNext.Location = new Point(4, 24);
        tabCallNext.Name = "tabCallNext";
        tabCallNext.Padding = new Padding(3);
        tabCallNext.Size = new Size(592, 432);
        tabCallNext.TabIndex = 0;
        tabCallNext.Text = "Дараагийн үйлчлүүлэгч";
        tabCallNext.UseVisualStyleBackColor = true;
        //
        // lblCallStatus
        //
        lblCallStatus.ForeColor = SystemColors.GrayText;
        lblCallStatus.Location = new Point(20, 400);
        lblCallStatus.Name = "lblCallStatus";
        lblCallStatus.Size = new Size(540, 25);
        lblCallStatus.TabIndex = 6;
        lblCallStatus.TextAlign = ContentAlignment.MiddleCenter;
        //
        // lblCallResult
        //
        lblCallResult.Font = new Font("Segoe UI", 12F, FontStyle.Bold);
        lblCallResult.Location = new Point(20, 340);
        lblCallResult.Name = "lblCallResult";
        lblCallResult.Size = new Size(540, 50);
        lblCallResult.TabIndex = 5;
        lblCallResult.TextAlign = ContentAlignment.MiddleCenter;
        //
        // btnCallNext
        //
        btnCallNext.BackColor = BankColors.Gold;
        btnCallNext.FlatAppearance.BorderSize = 0;
        btnCallNext.FlatStyle = FlatStyle.Flat;
        btnCallNext.Font = new Font("Segoe UI", 12F, FontStyle.Bold);
        btnCallNext.ForeColor = BankColors.Navy;
        btnCallNext.Location = new Point(20, 120);
        btnCallNext.Name = "btnCallNext";
        btnCallNext.Size = new Size(340, 50);
        btnCallNext.TabIndex = 4;
        btnCallNext.Text = "Next customer";
        btnCallNext.UseVisualStyleBackColor = false;
        //
        // btnRefreshNextWaiting
        //
        btnRefreshNextWaiting.FlatStyle = FlatStyle.Flat;
        btnRefreshNextWaiting.FlatAppearance.BorderColor = BankColors.Navy;
        btnRefreshNextWaiting.ForeColor = BankColors.Navy;
        btnRefreshNextWaiting.Location = new Point(430, 60);
        btnRefreshNextWaiting.Name = "btnRefreshNextWaiting";
        btnRefreshNextWaiting.Size = new Size(100, 28);
        btnRefreshNextWaiting.TabIndex = 3;
        btnRefreshNextWaiting.Text = "Шинэчлэх";
        btnRefreshNextWaiting.UseVisualStyleBackColor = true;
        //
        // lblNextWaiting
        //
        lblNextWaiting.Font = new Font("Segoe UI", 11F);
        lblNextWaiting.Location = new Point(20, 62);
        lblNextWaiting.Name = "lblNextWaiting";
        lblNextWaiting.Size = new Size(400, 25);
        lblNextWaiting.TabIndex = 2;
        lblNextWaiting.Text = "Хүлээгдэж буй дараагийн дугаар: —";
        //
        // numCounterNumber
        //
        numCounterNumber.Location = new Point(180, 20);
        numCounterNumber.Maximum = new decimal(new int[] { 99, 0, 0, 0 });
        numCounterNumber.Minimum = new decimal(new int[] { 1, 0, 0, 0 });
        numCounterNumber.Name = "numCounterNumber";
        numCounterNumber.Size = new Size(80, 27);
        numCounterNumber.TabIndex = 1;
        numCounterNumber.Value = new decimal(new int[] { 1, 0, 0, 0 });
        //
        // lblCounterNumber
        //
        lblCounterNumber.Location = new Point(20, 22);
        lblCounterNumber.Name = "lblCounterNumber";
        lblCounterNumber.Size = new Size(150, 25);
        lblCounterNumber.TabIndex = 0;
        lblCounterNumber.Text = "Цонхны дугаар:";
        //
        // tabTransfer
        //
        tabTransfer.Controls.Add(lblTransferResult);
        tabTransfer.Controls.Add(btnTransfer);
        tabTransfer.Controls.Add(txtAmount);
        tabTransfer.Controls.Add(lblAmount);
        tabTransfer.Controls.Add(lblToOwner);
        tabTransfer.Controls.Add(txtToAccount);
        tabTransfer.Controls.Add(lblTo);
        tabTransfer.Controls.Add(lblFromOwner);
        tabTransfer.Controls.Add(txtFromAccount);
        tabTransfer.Controls.Add(lblFrom);
        tabTransfer.BackColor = BankColors.Background;
        tabTransfer.Location = new Point(4, 24);
        tabTransfer.Name = "tabTransfer";
        tabTransfer.Padding = new Padding(3);
        tabTransfer.Size = new Size(592, 432);
        tabTransfer.TabIndex = 1;
        tabTransfer.Text = "Гүйлгээ";
        tabTransfer.UseVisualStyleBackColor = true;
        //
        // lblTransferResult
        //
        lblTransferResult.Font = new Font("Segoe UI", 10F, FontStyle.Bold);
        lblTransferResult.Location = new Point(20, 190);
        lblTransferResult.Name = "lblTransferResult";
        lblTransferResult.Size = new Size(540, 60);
        lblTransferResult.TabIndex = 9;
        //
        // btnTransfer
        //
        btnTransfer.BackColor = BankColors.Gold;
        btnTransfer.FlatAppearance.BorderSize = 0;
        btnTransfer.FlatStyle = FlatStyle.Flat;
        btnTransfer.Font = new Font("Segoe UI", 11F, FontStyle.Bold);
        btnTransfer.ForeColor = BankColors.Navy;
        btnTransfer.Location = new Point(20, 130);
        btnTransfer.Name = "btnTransfer";
        btnTransfer.Size = new Size(200, 45);
        btnTransfer.TabIndex = 8;
        btnTransfer.Text = "Шилжүүлэх";
        btnTransfer.UseVisualStyleBackColor = false;
        //
        // txtAmount
        //
        txtAmount.Location = new Point(210, 90);
        txtAmount.Name = "txtAmount";
        txtAmount.Size = new Size(150, 27);
        txtAmount.TabIndex = 7;
        //
        // lblAmount
        //
        lblAmount.Location = new Point(20, 93);
        lblAmount.Name = "lblAmount";
        lblAmount.Size = new Size(180, 25);
        lblAmount.TabIndex = 6;
        lblAmount.Text = "Дүн:";
        //
        // lblToOwner
        //
        lblToOwner.ForeColor = SystemColors.GrayText;
        lblToOwner.Location = new Point(370, 55);
        lblToOwner.Name = "lblToOwner";
        lblToOwner.Size = new Size(200, 25);
        lblToOwner.TabIndex = 5;
        //
        // txtToAccount
        //
        txtToAccount.Location = new Point(210, 55);
        txtToAccount.Name = "txtToAccount";
        txtToAccount.Size = new Size(150, 27);
        txtToAccount.TabIndex = 4;
        //
        // lblTo
        //
        lblTo.Location = new Point(20, 58);
        lblTo.Name = "lblTo";
        lblTo.Size = new Size(180, 25);
        lblTo.TabIndex = 3;
        lblTo.Text = "Дансны дугаар (Хүлээн авагч):";
        //
        // lblFromOwner
        //
        lblFromOwner.ForeColor = SystemColors.GrayText;
        lblFromOwner.Location = new Point(370, 20);
        lblFromOwner.Name = "lblFromOwner";
        lblFromOwner.Size = new Size(200, 25);
        lblFromOwner.TabIndex = 2;
        //
        // txtFromAccount
        //
        txtFromAccount.Location = new Point(210, 20);
        txtFromAccount.Name = "txtFromAccount";
        txtFromAccount.Size = new Size(150, 27);
        txtFromAccount.TabIndex = 1;
        //
        // lblFrom
        //
        lblFrom.Location = new Point(20, 23);
        lblFrom.Name = "lblFrom";
        lblFrom.Size = new Size(180, 25);
        lblFrom.TabIndex = 0;
        lblFrom.Text = "Дансны дугаар (Илгээгч):";
        //
        // tabRates
        //
        tabRates.Controls.Add(lblRateStatus);
        tabRates.Controls.Add(btnUpdateRate);
        tabRates.Controls.Add(txtSellRate);
        tabRates.Controls.Add(lblSellRate);
        tabRates.Controls.Add(txtBuyRate);
        tabRates.Controls.Add(lblBuyRate);
        tabRates.Controls.Add(txtCurrencyCode);
        tabRates.Controls.Add(lblCurrencyCode);
        tabRates.Controls.Add(btnRefreshRates);
        tabRates.Controls.Add(lvRates);
        tabRates.BackColor = BankColors.Background;
        tabRates.Location = new Point(4, 24);
        tabRates.Name = "tabRates";
        tabRates.Padding = new Padding(3);
        tabRates.Size = new Size(592, 432);
        tabRates.TabIndex = 2;
        tabRates.Text = "Валютын ханш";
        tabRates.UseVisualStyleBackColor = true;
        //
        // lblRateStatus
        //
        lblRateStatus.ForeColor = SystemColors.GrayText;
        lblRateStatus.Location = new Point(20, 380);
        lblRateStatus.Name = "lblRateStatus";
        lblRateStatus.Size = new Size(540, 25);
        lblRateStatus.TabIndex = 9;
        //
        // btnUpdateRate
        //
        btnUpdateRate.BackColor = BankColors.Gold;
        btnUpdateRate.FlatAppearance.BorderSize = 0;
        btnUpdateRate.FlatStyle = FlatStyle.Flat;
        btnUpdateRate.Font = new Font("Segoe UI", 10F, FontStyle.Bold);
        btnUpdateRate.ForeColor = BankColors.Navy;
        btnUpdateRate.Location = new Point(20, 335);
        btnUpdateRate.Name = "btnUpdateRate";
        btnUpdateRate.Size = new Size(160, 35);
        btnUpdateRate.TabIndex = 8;
        btnUpdateRate.Text = "Ханш өөрчлөх";
        btnUpdateRate.UseVisualStyleBackColor = false;
        //
        // txtSellRate
        //
        txtSellRate.Location = new Point(110, 295);
        txtSellRate.Name = "txtSellRate";
        txtSellRate.Size = new Size(100, 27);
        txtSellRate.TabIndex = 7;
        //
        // lblSellRate
        //
        lblSellRate.Location = new Point(20, 298);
        lblSellRate.Name = "lblSellRate";
        lblSellRate.Size = new Size(90, 25);
        lblSellRate.TabIndex = 6;
        lblSellRate.Text = "Зарах ханш:";
        //
        // txtBuyRate
        //
        txtBuyRate.Location = new Point(110, 260);
        txtBuyRate.Name = "txtBuyRate";
        txtBuyRate.Size = new Size(100, 27);
        txtBuyRate.TabIndex = 5;
        //
        // lblBuyRate
        //
        lblBuyRate.Location = new Point(20, 263);
        lblBuyRate.Name = "lblBuyRate";
        lblBuyRate.Size = new Size(90, 25);
        lblBuyRate.TabIndex = 4;
        lblBuyRate.Text = "Авах ханш:";
        //
        // txtCurrencyCode
        //
        txtCurrencyCode.CharacterCasing = CharacterCasing.Upper;
        txtCurrencyCode.Location = new Point(110, 225);
        txtCurrencyCode.MaxLength = 3;
        txtCurrencyCode.Name = "txtCurrencyCode";
        txtCurrencyCode.Size = new Size(100, 27);
        txtCurrencyCode.TabIndex = 3;
        //
        // lblCurrencyCode
        //
        lblCurrencyCode.Location = new Point(20, 228);
        lblCurrencyCode.Name = "lblCurrencyCode";
        lblCurrencyCode.Size = new Size(80, 25);
        lblCurrencyCode.TabIndex = 2;
        lblCurrencyCode.Text = "Валют:";
        //
        // btnRefreshRates
        //
        btnRefreshRates.FlatStyle = FlatStyle.Flat;
        btnRefreshRates.FlatAppearance.BorderColor = BankColors.Navy;
        btnRefreshRates.ForeColor = BankColors.Navy;
        btnRefreshRates.Location = new Point(20, 180);
        btnRefreshRates.Name = "btnRefreshRates";
        btnRefreshRates.Size = new Size(120, 30);
        btnRefreshRates.TabIndex = 1;
        btnRefreshRates.Text = "Шинэчлэх";
        btnRefreshRates.UseVisualStyleBackColor = true;
        //
        // lvRates
        //
        lvRates.Columns.Add(chCurrency);
        lvRates.Columns.Add(chBuy);
        lvRates.Columns.Add(chSell);
        lvRates.Columns.Add(chUpdated);
        lvRates.FullRowSelect = true;
        lvRates.GridLines = true;
        lvRates.Location = new Point(20, 20);
        lvRates.Name = "lvRates";
        lvRates.Size = new Size(540, 150);
        lvRates.TabIndex = 0;
        lvRates.UseCompatibleStateImageBehavior = false;
        lvRates.View = View.Details;
        //
        // chCurrency
        //
        chCurrency.Text = "Валют";
        chCurrency.Width = 80;
        //
        // chBuy
        //
        chBuy.Text = "Авах ханш";
        chBuy.Width = 120;
        //
        // chSell
        //
        chSell.Text = "Зарах ханш";
        chSell.Width = 120;
        //
        // chUpdated
        //
        chUpdated.Text = "Сүүлд шинэчлэгдсэн";
        chUpdated.Width = 200;
        //
        // TellerForm
        //
        AutoScaleMode = AutoScaleMode.Font;
        BackColor = BankColors.Background;
        ClientSize = new Size(620, 554);
        Controls.Add(tabMain);
        Controls.Add(pnlAccent);
        Controls.Add(pnlHeader);
        Name = "TellerForm";
        StartPosition = FormStartPosition.CenterScreen;
        Text = "Teller App";
        pnlHeader.ResumeLayout(false);
        tabMain.ResumeLayout(false);
        tabCallNext.ResumeLayout(false);
        ((System.ComponentModel.ISupportInitialize)numCounterNumber).EndInit();
        tabTransfer.ResumeLayout(false);
        tabTransfer.PerformLayout();
        tabRates.ResumeLayout(false);
        tabRates.PerformLayout();
        ResumeLayout(false);
    }

    #endregion

    private Panel pnlHeader;
    private Label lblFormTitle;
    private Panel pnlAccent;
    private TabControl tabMain;
    private TabPage tabCallNext;
    private Label lblCounterNumber;
    private NumericUpDown numCounterNumber;
    private Label lblNextWaiting;
    private Button btnRefreshNextWaiting;
    private Button btnCallNext;
    private Label lblCallResult;
    private Label lblCallStatus;
    private TabPage tabTransfer;
    private Label lblFrom;
    private TextBox txtFromAccount;
    private Label lblFromOwner;
    private Label lblTo;
    private TextBox txtToAccount;
    private Label lblToOwner;
    private Label lblAmount;
    private TextBox txtAmount;
    private Button btnTransfer;
    private Label lblTransferResult;
    private TabPage tabRates;
    private ListView lvRates;
    private ColumnHeader chCurrency;
    private ColumnHeader chBuy;
    private ColumnHeader chSell;
    private ColumnHeader chUpdated;
    private Button btnRefreshRates;
    private Label lblCurrencyCode;
    private TextBox txtCurrencyCode;
    private Label lblBuyRate;
    private TextBox txtBuyRate;
    private Label lblSellRate;
    private TextBox txtSellRate;
    private Button btnUpdateRate;
    private Label lblRateStatus;
}
