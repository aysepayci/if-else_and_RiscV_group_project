using RýscV_Generator;

namespace Comp_Org_Project
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }

        private void label1_Click(object sender, EventArgs e)
        {

        }

        private void button1_Click(object sender, EventArgs e)
        {
            label1.Text = "";

            string rawCode = textBox1.Text;

            var preProcessor = new CodePreProcessor();

            // 1. Adým: Satýrlarý temizle
            var cleanedCode = preProcessor.CleanCode(rawCode);

            // 2. Adým: Deðiþkenleri çýkar (gerekirse kullanýlabilir)
            string cleanedCodeString = string.Join("\n", cleanedCode);
            var variables = preProcessor.ExtractVariables(cleanedCodeString);

            // 3. Adým: RISC-V Kod Üretimi
            var generator = new RiscVCodeGenerator();
            var riscVCode = generator.CodeGenerator(preProcessor, rawCode);

            // Çýktýyý göster
            label1.Text = string.Join("\n", riscVCode);
        }


        private void textBox1_TextChanged(object sender, EventArgs e)
        {

        }

        private void Form1_Load(object sender, EventArgs e)
        {

        }
    }
}
