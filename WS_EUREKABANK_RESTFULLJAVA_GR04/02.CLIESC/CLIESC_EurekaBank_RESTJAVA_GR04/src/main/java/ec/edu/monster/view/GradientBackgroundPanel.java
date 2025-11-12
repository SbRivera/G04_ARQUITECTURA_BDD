package ec.edu.monster.view;

import javax.swing.*;
import java.awt.*;
import java.util.Random;

public class GradientBackgroundPanel extends JPanel {
  private final int[] cx, cy, r; // círculos suaves

  public GradientBackgroundPanel() {
    setOpaque(true);
    Random rnd = new Random(42);
    cx = new int[6]; cy = new int[6]; r = new int[6];
    for (int i=0;i<cx.length;i++){ cx[i]=100+rnd.nextInt(900); cy[i]=80+rnd.nextInt(400); r[i]=80+rnd.nextInt(120); }
  }

  @Override protected void paintComponent(Graphics g) {
    super.paintComponent(g);
    Graphics2D g2=(Graphics2D)g.create();
    g2.setRenderingHint(RenderingHints.KEY_ANTIALIASING, RenderingHints.VALUE_ANTIALIAS_ON);

    // Fondo azul degradado
    GradientPaint gp = new GradientPaint(0,0, Theme.NAVY, 0,getHeight(), Theme.NAVY_DARK);
    g2.setPaint(gp); g2.fillRect(0,0,getWidth(),getHeight());

    // Círculos suaves
    for(int i=0;i<cx.length;i++){
      g2.setColor(new Color(255,255,255,30));
      g2.fillOval(cx[i]-r[i]/2, cy[i]-r[i]/2, r[i], r[i]);
    }

    // Trama de puntos muy sutil
    g2.setColor(new Color(255,255,255,16));
    for(int x=0; x<getWidth(); x+=16)
      for(int y=0; y<getHeight(); y+=16)
        g2.fillOval(x, y, 2, 2);

    g2.dispose();
  }
}
