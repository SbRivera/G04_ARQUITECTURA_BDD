package ec.edu.monster.view;

import javax.swing.*;
import java.awt.*;
import java.util.function.Consumer;

public class TopBar extends JPanel {
  private final JButton btnLogout = new JButton("Cerrar Sesión");
  private final JLabel userBadge = new JLabel("MONSTER", SwingConstants.CENTER);

  public TopBar(){
    setOpaque(false);
    setLayout(new BorderLayout());

    JPanel left = new JPanel(){
      @Override protected void paintComponent(Graphics g){
        super.paintComponent(g);
        Graphics2D g2=(Graphics2D)g;
        g2.setPaint(Theme.headerGradient(getWidth()));
        g2.fillRect(0,getHeight()-4,getWidth(),4); // borde rojo
      }
    };
    left.setOpaque(false);
    left.setLayout(new FlowLayout(FlowLayout.LEFT, 16, 10));
    JLabel brand = new JLabel("<html><span style='font-weight:700;color:#d01522;font-size:18px'>EurekaBank</span> | Liga de Quito</html>");
    brand.setFont(Theme.H2);
    brand.setForeground(Color.DARK_GRAY);
    left.add(new JLabel("🏠 x ⭐⭐⭐⭐")); // placeholder logos
    left.add(brand);

    JPanel right = new JPanel(new FlowLayout(FlowLayout.RIGHT, 10, 6));
    right.setOpaque(false);
    userBadge.setOpaque(true);
    userBadge.setBackground(Theme.BLUE);
    userBadge.setForeground(Color.WHITE);
    userBadge.setBorder(BorderFactory.createEmptyBorder(8,14,8,14));
    userBadge.setFont(Theme.H2);

    btnLogout.setFocusPainted(false);
    btnLogout.setBackground(Theme.RED);
    btnLogout.setForeground(Color.WHITE);
    btnLogout.setBorder(BorderFactory.createEmptyBorder(8,14,8,14));

    add(left, BorderLayout.CENTER);
    add(right, BorderLayout.EAST);
    right.add(userBadge); right.add(btnLogout);
  }

  public void setUser(String u){ userBadge.setText(u.toUpperCase()); }
  public void onLogout(Runnable r){ btnLogout.addActionListener(e -> r.run()); }
}
