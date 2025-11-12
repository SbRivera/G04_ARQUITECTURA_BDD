package ec.edu.monster.view;

import javax.swing.*;
import java.awt.*;
import java.util.function.*;

public class MenuView extends JFrame {

  private final GradientBackgroundPanel bg = new GradientBackgroundPanel();
  private final TopBar top = new TopBar();

  private final CardView cardMov = new CardView("Consultar Movimientos", "📊");
  private final CardView cardDep = new CardView("Depósito", "💵");
  private final CardView cardRet = new CardView("Retiro", "👛");
  private final CardView cardTrf = new CardView("Transferencia", "💳");

  // mini-form para movimientos dentro de la card (como tu mock)
  private final JTextField txtCuentaMov = new RoundTextField(16);
  private final PrimaryButton btnVerMov = new PrimaryButton("VER MOVIMIENTOS");

  public MenuView(String usuario){
    super("EurekaBank | Liga de Quito");
    setContentPane(bg);
    setDefaultCloseOperation(DISPOSE_ON_CLOSE);
    setLayout(new BorderLayout());
    add(top, BorderLayout.NORTH);
    top.setUser(usuario);

    JPanel grid = new JPanel(new GridLayout(1,4,20,20));
    grid.setOpaque(false);
    grid.setBorder(BorderFactory.createEmptyBorder(28,52,28,52));
    grid.add(cardMov); grid.add(cardDep); grid.add(cardRet); grid.add(cardTrf);
    add(grid, BorderLayout.CENTER);

    // Contenido de la card de movimientos
    JPanel movForm = new JPanel(new GridBagLayout());
    movForm.setOpaque(false);
    GridBagConstraints c = new GridBagConstraints();
    c.insets = new Insets(4,4,4,4);
    c.gridx=0; c.gridy=0; c.anchor=GridBagConstraints.LINE_START;
    movForm.add(new JLabel("Número de cuenta:"), c);
    c.gridy=1; txtCuentaMov.setPreferredSize(new Dimension(220,36));
    ((RoundTextField)txtCuentaMov).setPlaceholder("Ingresa el número de cuenta");
    movForm.add(txtCuentaMov, c);
    c.gridy=2; c.insets = new Insets(12,4,4,4);
    movForm.add(btnVerMov, c);
    cardMov.setContent(movForm);

    setSize(1180, 680);
    setLocationRelativeTo(null);
  }

  public void onDeposito(Runnable r){ cardDep.addMouseListener(new java.awt.event.MouseAdapter(){ public void mouseClicked(java.awt.event.MouseEvent e){ r.run(); }}); }
  public void onRetiro(Runnable r){ cardRet.addMouseListener(new java.awt.event.MouseAdapter(){ public void mouseClicked(java.awt.event.MouseEvent e){ r.run(); }}); }
  public void onTransferencia(Runnable r){ cardTrf.addMouseListener(new java.awt.event.MouseAdapter(){ public void mouseClicked(java.awt.event.MouseEvent e){ r.run(); }}); }
  public void onMovimientos(Runnable r){ btnVerMov.addActionListener(e -> r.run()); }
  public String getCuentaMov(){ return txtCuentaMov.getText().trim(); }

  public void onSalir(Runnable r){ top.onLogout(r); }
}
