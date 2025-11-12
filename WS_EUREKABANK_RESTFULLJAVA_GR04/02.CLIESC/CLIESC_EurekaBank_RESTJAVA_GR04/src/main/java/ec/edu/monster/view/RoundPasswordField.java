package ec.edu.monster.view;

import javax.swing.*;
import javax.swing.border.EmptyBorder;
import java.awt.*;

public class RoundPasswordField extends JPasswordField {
  private String placeholder = "";
  public RoundPasswordField(int cols){ super(cols); setOpaque(false); setBorder(new EmptyBorder(8,12,8,12)); setFont(Theme.BODY); }
  public void setPlaceholder(String p){ this.placeholder = p; repaint(); }

  @Override protected void paintComponent(Graphics g) {
    Graphics2D g2=(Graphics2D)g.create();
    g2.setRenderingHint(RenderingHints.KEY_ANTIALIASING, RenderingHints.VALUE_ANTIALIAS_ON);
    int w=getWidth(), h=getHeight();
    g2.setColor(Color.WHITE);
    g2.fillRoundRect(0,0,w-1,h-1,16,16);
    g2.setColor(new Color(0,0,0,60));
    g2.drawRoundRect(0,0,w-1,h-1,16,16);
    super.paintComponent(g2);
    if(getPassword().length==0 && !isFocusOwner()){
      g2.setColor(Theme.TEXT_MUTED); g2.setFont(Theme.BODY);
      g2.drawString(placeholder, 12, getHeight()/2 + 5);
    }
    g2.dispose();
  }
}
