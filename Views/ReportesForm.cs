using DeforeStopDesktop.Config;
using DeforeStopDesktop.Controllers;
using DeforeStopDesktop.Views;
using DeforeStopDesktop.Models;
using System;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;

namespace DeforeStopDesktop.Views
{
    public partial class ReportesForm : Form
    {
        private DataGridView? dgvReportes;
        private System.ComponentModel.IContainer? components = null;
        private readonly ReporteController _controller = new ReporteController();

        public ReportesForm()
        {
            InitializeComponent();
            ConstruirUI();
            CargarReportes();
        }

        private void InitializeComponent()
        {
            this.components = new System.ComponentModel.Container();
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(1100, 750);
            this.Name = "ReportesForm";
            this.Text = "DeforeStop - Gestión de Reportes";
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        private void ConstruirUI()
        {
            this.Text = "DeforeStop - Gestión de Reportes";
            this.Size = new Size(1100, 600);
            this.StartPosition = FormStartPosition.CenterScreen;
            this.BackColor = Colores.FondoClaro;

            // 1. SplitContainer horizontal (Arriba tabla, abajo gráficos)
            SplitContainer splitVertical = new SplitContainer
            {
                Dock = DockStyle.Fill,
                Orientation = Orientation.Horizontal,
                SplitterDistance = 150,
                BackColor = Colores.FondoClaro
            };

            // --- TABLA (Arriba) ---
            dgvReportes = new DataGridView
            {
                Dock = DockStyle.Fill,
                BackgroundColor = Color.White,
                BorderStyle = BorderStyle.None,
                AllowUserToAddRows = false,
                AllowUserToDeleteRows = false,
                ReadOnly = true,
                SelectionMode = DataGridViewSelectionMode.FullRowSelect,
                MultiSelect = false,
                RowHeadersVisible = false,
                AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill,
                Font = new Font("Segoe UI", 10),
                ColumnHeadersHeight = 40
            };
            dgvReportes.RowTemplate.Height = 35;
            dgvReportes.ColumnHeadersDefaultCellStyle.BackColor = Colores.VerdeOscuro;
            dgvReportes.ColumnHeadersDefaultCellStyle.ForeColor = Color.White;
            dgvReportes.ColumnHeadersDefaultCellStyle.Font = new Font("Segoe UI", 10, FontStyle.Bold);
            dgvReportes.EnableHeadersVisualStyles = false;
            dgvReportes.AlternatingRowsDefaultCellStyle.BackColor = Color.FromArgb(240, 255, 240);
            dgvReportes.DefaultCellStyle.SelectionBackColor = Colores.VerdeNeon;
            dgvReportes.DefaultCellStyle.SelectionForeColor = Color.White;

            splitVertical.Panel1.Controls.Add(dgvReportes);

            // --- GRÁFICOS (Abajo) ---
            SplitContainer splitGraficos = new SplitContainer
            {
                Dock = DockStyle.Fill,
                Orientation = Orientation.Vertical,
                SplitterDistance = 550,
                BackColor = Colores.FondoClaro
            };

            // GRÁFICO 1 (Barras)
            Panel panelGrafico1 = new Panel { Dock = DockStyle.Fill, BackColor = Color.White, Padding = new Padding(20) };
            Label lblTitulo1 = new Label { Text = "📊 Hectáreas Deforestadas por Zona", Font = new Font("Segoe UI", 12, FontStyle.Bold), ForeColor = Colores.VerdeOscuro, Dock = DockStyle.Top, Height = 30 };
            panelGrafico1.Controls.Add(lblTitulo1);

            Panel panelBarras = new Panel { Dock = DockStyle.Fill, BackColor = Color.White, Name = "panelBarras" };
            panelGrafico1.Controls.Add(panelBarras);
            splitGraficos.Panel1.Controls.Add(panelGrafico1);

            // GRÁFICO 2 (Torta)
            Panel panelGrafico2 = new Panel { Dock = DockStyle.Fill, BackColor = Color.White, Padding = new Padding(20) };
            Label lblTitulo2 = new Label { Text = "🥧 Reportes Generados por Usuario", Font = new Font("Segoe UI", 12, FontStyle.Bold), ForeColor = Colores.VerdeOscuro, Dock = DockStyle.Top, Height = 30 };
            panelGrafico2.Controls.Add(lblTitulo2);

            Panel panelTorta = new Panel { Dock = DockStyle.Fill, BackColor = Color.White, Name = "panelTorta" };
            panelGrafico2.Controls.Add(panelTorta);
            splitGraficos.Panel2.Controls.Add(panelGrafico2);

            splitVertical.Panel2.Controls.Add(splitGraficos);
            this.Controls.Add(splitVertical);

            // 2. Panel de Botones
            Panel panelBotones = new Panel
            {
                Dock = DockStyle.Top,
                Height = 70,
                BackColor = Colores.FondoClaro,
                Padding = new Padding(20)
            };

            Button btnNuevo = new Button { Text = "➕ Nuevo Reporte", Font = new Font("Segoe UI", 11, FontStyle.Bold), Size = new Size(190, 45), Location = new Point(20, 15), BackColor = Colores.VerdeNeon, ForeColor = Color.White, FlatStyle = FlatStyle.Flat, Cursor = Cursors.Hand };
            btnNuevo.FlatAppearance.BorderSize = 0;
            btnNuevo.Click += (s, e) => AbrirEditor(null);
            panelBotones.Controls.Add(btnNuevo);

            Button btnEditar = new Button { Text = "✏️ Editar", Font = new Font("Segoe UI", 11, FontStyle.Bold), Size = new Size(140, 45), Location = new Point(230, 15), BackColor = Colores.Dorado, ForeColor = Color.White, FlatStyle = FlatStyle.Flat, Cursor = Cursors.Hand };
            btnEditar.FlatAppearance.BorderSize = 0;
            btnEditar.Click += (s, e) => EditarSeleccionado();
            panelBotones.Controls.Add(btnEditar);

            Button btnEliminar = new Button { Text = "🗑️ Eliminar", Font = new Font("Segoe UI", 11, FontStyle.Bold), Size = new Size(140, 45), Location = new Point(390, 15), BackColor = Colores.Rojo, ForeColor = Color.White, FlatStyle = FlatStyle.Flat, Cursor = Cursors.Hand };
            btnEliminar.FlatAppearance.BorderSize = 0;
            btnEliminar.Click += (s, e) => EliminarSeleccionado();
            panelBotones.Controls.Add(btnEliminar);

            this.Controls.Add(panelBotones);

            // 3. Navbar
            Panel navbar = new Panel { Dock = DockStyle.Top, Height = 70, BackColor = Colores.VerdeOscuro };
            Label lblTitulo = new Label { Text = "📊 Gestión de Reportes", Font = new Font("Segoe UI", 20, FontStyle.Bold), ForeColor = Color.White, AutoSize = true, Location = new Point(20, 20), BackColor = Color.Transparent };
            navbar.Controls.Add(lblTitulo);
            this.Controls.Add(navbar);
        }

        private void CargarReportes()
        {
            try
            {
                var reportes = _controller.ObtenerTodos();
                dgvReportes!.DataSource = reportes;
                dgvReportes.Columns["Id"]!.HeaderText = "ID";
                dgvReportes.Columns["Titulo"]!.HeaderText = "Título";
                dgvReportes.Columns["FechaInicio"]!.HeaderText = "Fecha Inicio";
                dgvReportes.Columns["FechaFin"]!.HeaderText = "Fecha Fin";
                dgvReportes.Columns["ZonaId"]!.HeaderText = "Zona ID";
                dgvReportes.Columns["UsuarioId"]!.HeaderText = "Usuario ID";
                dgvReportes.Columns["FechaGeneracion"]!.HeaderText = "Fecha Generación";

                // Enlazar eventos de dibujo
                var panelBarras = this.Controls.Find("panelBarras", true).FirstOrDefault() as Panel;
                var panelTorta = this.Controls.Find("panelTorta", true).FirstOrDefault() as Panel;

                if (panelBarras != null)
                {
                    panelBarras.Paint -= (s, e) => DibujarBarras(e.Graphics, panelBarras);
                    panelBarras.Paint += (s, e) => DibujarBarras(e.Graphics, panelBarras);
                    panelBarras.Invalidate();
                }

                if (panelTorta != null)
                {
                    panelTorta.Paint -= (s, e) => DibujarTorta(e.Graphics, panelTorta);
                    panelTorta.Paint += (s, e) => DibujarTorta(e.Graphics, panelTorta);
                    panelTorta.Invalidate();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error al cargar reportes: " + ex.Message, "Error",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void DibujarBarras(Graphics g, Panel panel)
        {
            using (var db = new AppDbContext())
            {
                var zonas = db.ZonasDeforestadas.ToList();
                if (zonas.Count == 0) return;

                decimal maxHectareas = zonas.Max(z => z.Hectareas);
                int anchoBarra = panel.Width / (zonas.Count * 2 + 1);
                int x = anchoBarra;

                foreach (var z in zonas)
                {
                    var nombreZona = db.Zonas.Find(z.ZonaId)?.Nombre ?? "N/A";
                    int altoBarra = (int)((z.Hectareas / maxHectareas) * (panel.Height - 40));
                    int y = panel.Height - altoBarra - 20;

                    g.FillRectangle(new SolidBrush(Colores.VerdeNeon), x, y, anchoBarra, altoBarra);
                    g.DrawString(z.Hectareas.ToString("0"), new Font("Segoe UI", 9, FontStyle.Bold), Brushes.Black, x, y - 20);
                    g.DrawString(nombreZona, new Font("Segoe UI", 8), Brushes.Black, x, panel.Height - 20);

                    x += anchoBarra * 2;
                }
            }
        }

        private void DibujarTorta(Graphics g, Panel panel)
        {
            using (var db = new AppDbContext())
            {
                var agrupados = db.Reportes
                    .GroupBy(r => r.UsuarioId)
                    .Select(gr => new { UsuarioId = gr.Key, Cantidad = gr.Count() })
                    .ToList();

                if (agrupados.Count == 0) return;

                int total = agrupados.Sum(a => a.Cantidad);
                float anguloInicio = 0f;
                Color[] colores = { Colores.VerdeNeon, Colores.Dorado, Colores.VerdeOscuro, Color.Orange, Color.Blue };

                int i = 0;
                foreach (var item in agrupados)
                {
                    float angulo = (item.Cantidad / (float)total) * 360f;
                    g.FillPie(new SolidBrush(colores[i % colores.Length]), 50, 20, 200, 200, anguloInicio, angulo);
                    anguloInicio += angulo;
                    i++;
                }
            }
        }

        private void AbrirEditor(Reporte? reporte)
        {
            ReporteEditForm editor = new ReporteEditForm(reporte);
            if (editor.ShowDialog() == DialogResult.OK)
            {
                CargarReportes();
            }
        }

        private void EditarSeleccionado()
        {
            if (dgvReportes!.SelectedRows.Count == 0)
            {
                MessageBox.Show("Selecciona un reporte para editar.", "Aviso",
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            int id = (int)dgvReportes.SelectedRows[0].Cells["Id"].Value;
            var reporte = _controller.ObtenerPorId(id);
            if (reporte != null) AbrirEditor(reporte);
        }

        private void EliminarSeleccionado()
        {
            if (dgvReportes!.SelectedRows.Count == 0)
            {
                MessageBox.Show("Selecciona un reporte para eliminar.", "Aviso",
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            int id = (int)dgvReportes.SelectedRows[0].Cells["Id"].Value;
            string titulo = dgvReportes.SelectedRows[0].Cells["Titulo"].Value?.ToString() ?? "";

            if (MessageBox.Show($"¿Eliminar el reporte \"{titulo}\"?", "Confirmar",
                MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
            {
                if (_controller.Eliminar(id))
                {
                    CargarReportes();
                    MessageBox.Show("Reporte eliminado correctamente.", "Éxito",
                        MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
                else
                {
                    MessageBox.Show("Error al eliminar reporte.", "Error",
                        MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
        }
    }
}