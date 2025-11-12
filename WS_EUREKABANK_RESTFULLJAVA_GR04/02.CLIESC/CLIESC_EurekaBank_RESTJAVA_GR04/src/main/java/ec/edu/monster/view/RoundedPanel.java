package ec.edu.monster.view;

import javax.swing.*;
import java.awt.*;

public class RoundedPanel extends JPanel {
  private int radius = 20;
  public RoundedPanel(){ setOpaque(false); }
  public RoundedPanel(int radius){ this(); this.radius = radius; }

  @Override protected void paintComponent(Graphics g) {
    int w = getWidth(), h = getHeight();
    Graphics2D g2=(Graphics2D)g.create();
    g2.setRenderingHint(RenderingHints.KEY_ANTIALIASING, RenderingHints.VALUE_ANTIALIAS_ON);

    // sombra
    g2.setColor(Theme.CARD_SHADOW);
    g2.fillRoundRect(4,6,w-8,h-8,radius+10,radius+10);

    // cuerpo
    g2.setColor(Theme.CARD_BG);
    g2.fillRoundRect(0,0,w-8,h-8,radius,radius);

    g2.dispose();
    super.paintComponent(g);
  }
}
