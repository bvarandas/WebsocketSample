namespace WinFormTest
{
    partial class frmPositionClient
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
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
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.btnInicia = new System.Windows.Forms.Button();
            this.btnFinaliza = new System.Windows.Forms.Button();
            this.btnFiltroOperacaoIntraday = new System.Windows.Forms.Button();
            this.btnEfetuaFiltroRiscoResumido = new System.Windows.Forms.Button();
            this.btnIniciaRiscoResumido = new System.Windows.Forms.Button();
            this.btnFinalizaRiscoResumido = new System.Windows.Forms.Button();
            this.btnIniciaRiscoResumidoIntranet = new System.Windows.Forms.Button();
            this.btnFinalizaRiscoResumidoIntranet = new System.Windows.Forms.Button();
            this.SuspendLayout();
            // 
            // btnInicia
            // 
            this.btnInicia.Location = new System.Drawing.Point(31, 10);
            this.btnInicia.Name = "btnInicia";
            this.btnInicia.Size = new System.Drawing.Size(187, 23);
            this.btnInicia.TabIndex = 0;
            this.btnInicia.Text = "Inicia Serviço Operações Intraday";
            this.btnInicia.UseVisualStyleBackColor = true;
            this.btnInicia.Click += new System.EventHandler(this.btnInicia_Click);
            // 
            // btnFinaliza
            // 
            this.btnFinaliza.Location = new System.Drawing.Point(31, 92);
            this.btnFinaliza.Name = "btnFinaliza";
            this.btnFinaliza.Size = new System.Drawing.Size(187, 23);
            this.btnFinaliza.TabIndex = 1;
            this.btnFinaliza.Text = "Finaliza Serviço Operações Intraday";
            this.btnFinaliza.UseVisualStyleBackColor = true;
            this.btnFinaliza.Click += new System.EventHandler(this.btnFinaliza_Click);
            // 
            // btnFiltroOperacaoIntraday
            // 
            this.btnFiltroOperacaoIntraday.Location = new System.Drawing.Point(31, 190);
            this.btnFiltroOperacaoIntraday.Name = "btnFiltroOperacaoIntraday";
            this.btnFiltroOperacaoIntraday.Size = new System.Drawing.Size(187, 23);
            this.btnFiltroOperacaoIntraday.TabIndex = 2;
            this.btnFiltroOperacaoIntraday.Text = "Testar Filtro Operacoes Intraday";
            this.btnFiltroOperacaoIntraday.UseVisualStyleBackColor = true;
            this.btnFiltroOperacaoIntraday.Click += new System.EventHandler(this.btnFiltroOperacaoIntraday_Click);
            // 
            // btnEfetuaFiltroRiscoResumido
            // 
            this.btnEfetuaFiltroRiscoResumido.Location = new System.Drawing.Point(31, 219);
            this.btnEfetuaFiltroRiscoResumido.Name = "btnEfetuaFiltroRiscoResumido";
            this.btnEfetuaFiltroRiscoResumido.Size = new System.Drawing.Size(187, 23);
            this.btnEfetuaFiltroRiscoResumido.TabIndex = 3;
            this.btnEfetuaFiltroRiscoResumido.Text = "Testar Filtro Risco Resumido";
            this.btnEfetuaFiltroRiscoResumido.UseVisualStyleBackColor = true;
            this.btnEfetuaFiltroRiscoResumido.Click += new System.EventHandler(this.btnEfetuaFiltroRiscoResumido_Click);
            // 
            // btnIniciaRiscoResumido
            // 
            this.btnIniciaRiscoResumido.Location = new System.Drawing.Point(31, 34);
            this.btnIniciaRiscoResumido.Name = "btnIniciaRiscoResumido";
            this.btnIniciaRiscoResumido.Size = new System.Drawing.Size(187, 23);
            this.btnIniciaRiscoResumido.TabIndex = 4;
            this.btnIniciaRiscoResumido.Text = "Inicia Serviço Risco Resumido";
            this.btnIniciaRiscoResumido.UseVisualStyleBackColor = true;
            this.btnIniciaRiscoResumido.Click += new System.EventHandler(this.btnIniciaRiscoResumido_Click);
            // 
            // btnFinalizaRiscoResumido
            // 
            this.btnFinalizaRiscoResumido.Location = new System.Drawing.Point(31, 115);
            this.btnFinalizaRiscoResumido.Name = "btnFinalizaRiscoResumido";
            this.btnFinalizaRiscoResumido.Size = new System.Drawing.Size(187, 23);
            this.btnFinalizaRiscoResumido.TabIndex = 5;
            this.btnFinalizaRiscoResumido.Text = "Finaliza Serviço Risco Resumido";
            this.btnFinalizaRiscoResumido.UseVisualStyleBackColor = true;
            this.btnFinalizaRiscoResumido.Click += new System.EventHandler(this.btnFinalizaRiscoResumido_Click);
            // 
            // btnIniciaRiscoResumidoIntranet
            // 
            this.btnIniciaRiscoResumidoIntranet.Location = new System.Drawing.Point(31, 58);
            this.btnIniciaRiscoResumidoIntranet.Name = "btnIniciaRiscoResumidoIntranet";
            this.btnIniciaRiscoResumidoIntranet.Size = new System.Drawing.Size(187, 23);
            this.btnIniciaRiscoResumidoIntranet.TabIndex = 6;
            this.btnIniciaRiscoResumidoIntranet.Text = "Inicia Risco Resumido Intranet";
            this.btnIniciaRiscoResumidoIntranet.UseVisualStyleBackColor = true;
            this.btnIniciaRiscoResumidoIntranet.Click += new System.EventHandler(this.btnIniciaRiscoResumidoIntranet_Click);
            // 
            // btnFinalizaRiscoResumidoIntranet
            // 
            this.btnFinalizaRiscoResumidoIntranet.Location = new System.Drawing.Point(31, 138);
            this.btnFinalizaRiscoResumidoIntranet.Name = "btnFinalizaRiscoResumidoIntranet";
            this.btnFinalizaRiscoResumidoIntranet.Size = new System.Drawing.Size(187, 23);
            this.btnFinalizaRiscoResumidoIntranet.TabIndex = 7;
            this.btnFinalizaRiscoResumidoIntranet.Text = "Finaliza Risco Resumido Intranet";
            this.btnFinalizaRiscoResumidoIntranet.UseVisualStyleBackColor = true;
            // 
            // frmPositionClient
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(241, 282);
            this.Controls.Add(this.btnFinalizaRiscoResumidoIntranet);
            this.Controls.Add(this.btnIniciaRiscoResumidoIntranet);
            this.Controls.Add(this.btnFinalizaRiscoResumido);
            this.Controls.Add(this.btnIniciaRiscoResumido);
            this.Controls.Add(this.btnEfetuaFiltroRiscoResumido);
            this.Controls.Add(this.btnFiltroOperacaoIntraday);
            this.Controls.Add(this.btnFinaliza);
            this.Controls.Add(this.btnInicia);
            this.Name = "frmPositionClient";
            this.Text = "Teste Position Client";
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.frmPositionClient_FormClosing);
            this.Load += new System.EventHandler(this.frmPositionClient_Load);
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Button btnInicia;
        private System.Windows.Forms.Button btnFinaliza;
        private System.Windows.Forms.Button btnFiltroOperacaoIntraday;
        private System.Windows.Forms.Button btnEfetuaFiltroRiscoResumido;
        private System.Windows.Forms.Button btnIniciaRiscoResumido;
        private System.Windows.Forms.Button btnFinalizaRiscoResumido;
        private System.Windows.Forms.Button btnIniciaRiscoResumidoIntranet;
        private System.Windows.Forms.Button btnFinalizaRiscoResumidoIntranet;
    }
}

