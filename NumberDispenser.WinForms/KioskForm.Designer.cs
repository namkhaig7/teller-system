namespace NumberDispenser.WinForms;

partial class KioskForm
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
        lblTitle = new Label();
        pnlAccent = new Panel();
        pnlContent = new Panel();
        lblStatus = new Label();
        pnlServing = new Panel();
        lblNowServing = new Label();
        pnlNumberCard = new Panel();
        lblYourNumber = new Label();
        lblYourNumberCaption = new Label();
        btnTakeNumber = new Button();
        pnlHeader.SuspendLayout();
        pnlContent.SuspendLayout();
        pnlServing.SuspendLayout();
        pnlNumberCard.SuspendLayout();
        SuspendLayout();
        //
        // pnlHeader
        //
        pnlHeader.BackColor = BankColors.Navy;
        pnlHeader.Controls.Add(lblTitle);
        pnlHeader.Dock = DockStyle.Top;
        pnlHeader.Location = new Point(0, 0);
        pnlHeader.Name = "pnlHeader";
        pnlHeader.Size = new Size(520, 90);
        pnlHeader.TabIndex = 0;
        //
        // lblTitle
        //
        lblTitle.Dock = DockStyle.Fill;
        lblTitle.Font = new Font("Segoe UI", 17F, FontStyle.Bold);
        lblTitle.ForeColor = Color.White;
        lblTitle.Location = new Point(0, 0);
        lblTitle.Name = "lblTitle";
        lblTitle.Size = new Size(520, 90);
        lblTitle.TabIndex = 0;
        lblTitle.Text = "Дугаар олгогч";
        lblTitle.TextAlign = ContentAlignment.MiddleCenter;
        //
        // pnlAccent
        //
        pnlAccent.BackColor = BankColors.Gold;
        pnlAccent.Dock = DockStyle.Top;
        pnlAccent.Location = new Point(0, 90);
        pnlAccent.Name = "pnlAccent";
        pnlAccent.Size = new Size(520, 4);
        pnlAccent.TabIndex = 1;
        //
        // pnlContent
        //
        pnlContent.BackColor = BankColors.Background;
        pnlContent.Controls.Add(lblStatus);
        pnlContent.Controls.Add(pnlServing);
        pnlContent.Controls.Add(pnlNumberCard);
        pnlContent.Controls.Add(btnTakeNumber);
        pnlContent.Dock = DockStyle.Fill;
        pnlContent.Location = new Point(0, 94);
        pnlContent.Name = "pnlContent";
        pnlContent.Size = new Size(520, 386);
        pnlContent.TabIndex = 2;
        //
        // lblStatus
        //
        lblStatus.Font = new Font("Segoe UI", 8F);
        lblStatus.ForeColor = SystemColors.GrayText;
        lblStatus.Location = new Point(20, 330);
        lblStatus.Name = "lblStatus";
        lblStatus.Size = new Size(480, 20);
        lblStatus.TabIndex = 3;
        lblStatus.Text = "Сервертэй холбогдож байна...";
        lblStatus.TextAlign = ContentAlignment.MiddleCenter;
        //
        // pnlServing
        //
        pnlServing.BackColor = BankColors.ServingPill;
        pnlServing.Controls.Add(lblNowServing);
        pnlServing.Location = new Point(60, 265);
        pnlServing.Name = "pnlServing";
        pnlServing.Size = new Size(400, 45);
        pnlServing.TabIndex = 2;
        //
        // lblNowServing
        //
        lblNowServing.Dock = DockStyle.Fill;
        lblNowServing.Font = new Font("Segoe UI", 11F, FontStyle.Bold);
        lblNowServing.ForeColor = BankColors.Navy;
        lblNowServing.Location = new Point(0, 0);
        lblNowServing.Name = "lblNowServing";
        lblNowServing.Size = new Size(400, 45);
        lblNowServing.TabIndex = 0;
        lblNowServing.Text = "Одоо үйлчилж буй: —";
        lblNowServing.TextAlign = ContentAlignment.MiddleCenter;
        //
        // pnlNumberCard
        //
        pnlNumberCard.BackColor = Color.White;
        pnlNumberCard.BorderStyle = BorderStyle.FixedSingle;
        pnlNumberCard.Controls.Add(lblYourNumber);
        pnlNumberCard.Controls.Add(lblYourNumberCaption);
        pnlNumberCard.Location = new Point(60, 110);
        pnlNumberCard.Name = "pnlNumberCard";
        pnlNumberCard.Size = new Size(400, 140);
        pnlNumberCard.TabIndex = 1;
        //
        // lblYourNumber
        //
        lblYourNumber.Font = new Font("Segoe UI", 42F, FontStyle.Bold);
        lblYourNumber.ForeColor = BankColors.Navy;
        lblYourNumber.Location = new Point(0, 40);
        lblYourNumber.Name = "lblYourNumber";
        lblYourNumber.Size = new Size(398, 90);
        lblYourNumber.TabIndex = 1;
        lblYourNumber.Text = "—";
        lblYourNumber.TextAlign = ContentAlignment.MiddleCenter;
        //
        // lblYourNumberCaption
        //
        lblYourNumberCaption.ForeColor = SystemColors.GrayText;
        lblYourNumberCaption.Location = new Point(0, 12);
        lblYourNumberCaption.Name = "lblYourNumberCaption";
        lblYourNumberCaption.Size = new Size(398, 25);
        lblYourNumberCaption.TabIndex = 0;
        lblYourNumberCaption.Text = "Таны дугаар:";
        lblYourNumberCaption.TextAlign = ContentAlignment.MiddleCenter;
        //
        // btnTakeNumber
        //
        btnTakeNumber.BackColor = BankColors.Gold;
        btnTakeNumber.FlatAppearance.BorderSize = 0;
        btnTakeNumber.FlatStyle = FlatStyle.Flat;
        btnTakeNumber.Font = new Font("Segoe UI", 14F, FontStyle.Bold);
        btnTakeNumber.ForeColor = BankColors.Navy;
        btnTakeNumber.Location = new Point(110, 20);
        btnTakeNumber.Name = "btnTakeNumber";
        btnTakeNumber.Size = new Size(300, 70);
        btnTakeNumber.TabIndex = 0;
        btnTakeNumber.Text = "Дугаар авах";
        btnTakeNumber.UseVisualStyleBackColor = false;
        //
        // KioskForm
        //
        AutoScaleMode = AutoScaleMode.Font;
        BackColor = BankColors.Background;
        ClientSize = new Size(520, 480);
        Controls.Add(pnlContent);
        Controls.Add(pnlAccent);
        Controls.Add(pnlHeader);
        Name = "KioskForm";
        StartPosition = FormStartPosition.CenterScreen;
        Text = "Dugaar Olgogch";
        pnlHeader.ResumeLayout(false);
        pnlContent.ResumeLayout(false);
        pnlServing.ResumeLayout(false);
        pnlNumberCard.ResumeLayout(false);
        ResumeLayout(false);
    }

    #endregion

    private Panel pnlHeader;
    private Label lblTitle;
    private Panel pnlAccent;
    private Panel pnlContent;
    private Button btnTakeNumber;
    private Panel pnlNumberCard;
    private Label lblYourNumberCaption;
    private Label lblYourNumber;
    private Panel pnlServing;
    private Label lblNowServing;
    private Label lblStatus;
}
