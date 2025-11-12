package ec.edu.monster.view;

import javax.swing.*;
import java.awt.*;

public class PrimaryButton extends JButton {
  public PrimaryButton(String text){ super(text); setContentAreaFilled(false); setFocusPainted(false); setForeground(Color.WHITE); setFont(Theme.H2); }
  @Override protected void paintComponent(Graphics g) {
    Graphics2D g2=(Graphics2D)g.create();
    g2.setRenderingHint(RenderingHints.KEY_ANTIALIASING, RenderingHints.VALUE_ANTIALIAS_ON);
    int w=getWidth(), h=getHeight();
    GradientPaint gp = new GradientPaint(0,0, Theme.RED, 0,h, Theme.RED_DARK);
    g2.setPaint(gp); g2.fillRoundRect(0,0,w,h,24,24);
    g2.dispose();
    super.paintComponent(g);
  }
  @Override public Insets getInsets(){ return new Insets(10,18,10,18); }
}
