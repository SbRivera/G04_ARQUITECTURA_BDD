package ec.edu.monster.view;

import javax.swing.*;
import java.awt.*;
import java.util.function.BiConsumer;

public class LoginView extends JFrame {
  private final GradientBackgroundPanel bg = new GradientBackgroundPanel();
  private final RoundedPanel card = new RoundedPanel(28);
  private final RoundTextField txtUser = new RoundTextField(18);
  private final RoundPasswordField txtPass = new RoundPasswordField(18);
  private final PrimaryButton btnLogin = new PrimaryButton("INGRESAR");

  public LoginView(){
    super("EurekaBank | Liga de Quito – Login");
    setContentPane(bg);
    setDefaultCloseOperation(EXIT_ON_CLOSE);
    setLayout(new GridBagLayout());
    GridBagConstraints g = new GridBagConstraints();
    g.insets = new Insets(16,16,16,16);
    g.gridx=0; g.gridy=0;
    add(buildCard(), g);
    setSize(980,580);
    setLocationRelativeTo(null);
  }

  private JPanel buildCard(){
    card.setLayout(new GridBagLayout());
    GridBagConstraints c = new GridBagConstraints();
    c.insets = new Insets(8,12,8,12);
    c.gridx=0; c.gridy=0; c.gridwidth=2;
    // Ícono circular
    card.add(new IconBadge(64, Theme.RED), c);

    c.gridy=1;
    JLabel title = new JLabel("Ingreso al sistema");
    title.setFont(Theme.H1);
    title.setForeground(Theme.RED_DARK);
    card.add(title, c);

    c.gridwidth=1; c.gridy=2; c.gridx=0;
    txtUser.setPlaceholder("Usuario");
    card.add(txtUser, c);

    c.gridx=1;
    txtPass.setPlaceholder("Contraseña");
    card.add(txtPass, c);

    c.gridx=0; c.gridy=3; c.gridwidth=2; c.insets = new Insets(18,12,8,12);
    card.add(btnLogin, c);

    return card;
  }

  public void onLoginClick(BiConsumer<String,String> h){
    btnLogin.addActionListener(e -> h.accept(txtUser.getText().trim(), new String(txtPass.getPassword())));
    getRootPane().setDefaultButton(btnLogin);
  }

  // Badge circular simple
  static class IconBadge extends JComponent {
    private final int size; private final Color color;
    IconBadge(int size, Color color){ this.size=size; this.color=color; setPreferredSize(new Dimension(size,size)); }
    @Override protected void paintComponent(Graphics g) {
      Graphics2D g2=(Graphics2D)g.create();
      g2.setRenderingHint(RenderingHints.KEY_ANTIALIASING, RenderingHints.VALUE_ANTIALIAS_ON);
      g2.setColor(color); g2.fillOval(0,0,size,size);
      g2.setColor(Color.WHITE); int r=size/3;
      g2.fillOval(size/2 - r/2, size/4, r, r);
      g2.fillRoundRect(size/2 - r, size/2, 2*r, r+6, r, r);
      g2.dispose();
    }
  }
}
