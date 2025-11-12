package ec.edu.monster.view;

import javax.swing.*;
import javax.swing.table.DefaultTableCellRenderer;
import javax.swing.table.DefaultTableModel;
import java.awt.*;
import java.util.function.Consumer;

public class MovimientosView extends JDialog {

  private final JLabel lblCuenta = new JLabel(" — ");
  private final JTable tabla = new JTable();
  private final RoundTextField txtCuenta = new RoundTextField(16);
  private final PrimaryButton btnBuscar = new PrimaryButton("BUSCAR");

  public MovimientosView(Window owner){
    super(owner, "Movimientos", ModalityType.APPLICATION_MODAL);
    setContentPane(new GradientBackgroundPanel());
    setLayout(new GridBagLayout());

    RoundedPanel card = new RoundedPanel(24);
    card.setLayout(new BorderLayout());
    card.setBorder(BorderFactory.createEmptyBorder(10,10,10,10));

    // Header estilizado
    JPanel header = new JPanel(){
      @Override protected void paintComponent(Graphics g){
        super.paintComponent(g);
        Graphics2D g2=(Graphics2D)g.create();
        g2.setPaint(Theme.headerGradient(getWidth())); g2.fillRoundRect(0,0,getWidth(),80,18,18);
        g2.dispose();
      }
    };
    header.setOpaque(false);
    header.setLayout(new GridBagLayout());
    JLabel icon = new JLabel("📊", SwingConstants.CENTER);
    icon.setFont(new Font("Segoe UI Emoji", Font.PLAIN, 36));
    JLabel title = new JLabel("Movimientos de Cuenta");
    title.setFont(Theme.H1); title.setForeground(Theme.RED_DARK);

    GridBagConstraints h=new GridBagConstraints();
    h.insets=new Insets(6,6,6,6); h.gridx=0; header.add(icon,h);
    h.gridx=1; header.add(title,h);
    h.gridx=2; lblCuenta.setOpaque(true); lblCuenta.setBackground(Theme.BLUE); lblCuenta.setForeground(Color.WHITE);
    lblCuenta.setBorder(BorderFactory.createEmptyBorder(6,14,6,14));
    lblCuenta.setFont(Theme.H2);
    header.add(lblCuenta,h);
    card.add(header, BorderLayout.NORTH);

    // Tabla
    tabla.setFillsViewportHeight(true);
    tabla.getTableHeader().setFont(Theme.H2);
    tabla.getTableHeader().setBackground(Theme.RED);
    tabla.getTableHeader().setForeground(Color.WHITE);
    tabla.setRowHeight(28);
    ((DefaultTableCellRenderer)tabla.getTableHeader().getDefaultRenderer()).setHorizontalAlignment(SwingConstants.LEFT);
    card.add(new JScrollPane(tabla), BorderLayout.CENTER);

    // Filtro inferior opcional
    JPanel bottom = new JPanel(new FlowLayout(FlowLayout.LEFT, 10, 8));
    bottom.setOpaque(false);
    txtCuenta.setPlaceholder("Número de cuenta");
    bottom.add(txtCuenta); bottom.add(btnBuscar);
    card.add(bottom, BorderLayout.SOUTH);

    add(card, new GridBagConstraints());
    setSize(900, 540);
    setLocationRelativeTo(owner);
  }

  public void onBuscar(Consumer<String> h){
    btnBuscar.addActionListener(e -> h.accept(txtCuenta.getText().trim()));
  }
  public void setTableModel(DefaultTableModel m){ tabla.setModel(m); }
  public void setCuentaBadge(String c){ lblCuenta.setText(c); }
}
