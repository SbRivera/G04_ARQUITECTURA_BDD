package ec.edu.monster.view;

import javax.swing.*;
import java.awt.*;

public final class Dialogs {
  private Dialogs(){}

  public static void success(Window parent, String msg){
    JOptionPane pane = new JOptionPane(
        new StyledMessage("¡Operación Exitosa!", msg, true),
        JOptionPane.PLAIN_MESSAGE, JOptionPane.DEFAULT_OPTION);
    JDialog dlg = pane.createDialog(parent, "");
    dlg.setModal(true);
    dlg.setSize(520, 280);
    dlg.setLocationRelativeTo(parent);
    dlg.setVisible(true);
  }

  public static void error(Window parent, String msg){
    JOptionPane pane = new JOptionPane(
        new StyledMessage("Error en la Operación", msg, false),
        JOptionPane.PLAIN_MESSAGE, JOptionPane.DEFAULT_OPTION);
    JDialog dlg = pane.createDialog(parent, "");
    dlg.setModal(true);
    dlg.setSize(520, 280);
    dlg.setLocationRelativeTo(parent);
    dlg.setVisible(true);
  }

  static class StyledMessage extends JPanel {
    private final String title, msg; private final boolean ok;
    StyledMessage(String title, String msg, boolean ok){ this.title=title; this.msg=msg; this.ok=ok; setOpaque(false); setPreferredSize(new Dimension(500,240)); }
    @Override protected void paintComponent(Graphics g){
      super.paintComponent(g);
      Graphics2D g2=(Graphics2D)g.create();
      g2.setRenderingHint(RenderingHints.KEY_ANTIALIASING, RenderingHints.VALUE_ANTIALIAS_ON);
      int w=getWidth(), h=getHeight();
      g2.setColor(new Color(255,255,255,230)); g2.fillRoundRect(10,10,w-20,h-20,26,26);
      g2.setColor(new Color(0,0,0,35)); g2.drawRoundRect(10,10,w-20,h-20,26,26);

      // ícono
      g2.setColor(ok? new Color(36, 161, 79): Theme.RED);
      int r=56; g2.fillOval(w/2 - r/2, 28, r, r);
      g2.setColor(Color.WHITE);
      if(ok){ // check
        int x = w/2 - 14, y = 52;
        g2.fillPolygon(new int[]{x, x+10, x+28, x+22, x+10, x+4}, new int[]{y+10,y+22,y-2,y-8,y+12,y+6}, 6);
      } else { // X
        g2.fillRoundRect(w/2-14, 56, 28, 6, 6,6);
        g2.fillRoundRect(w/2-3, 45, 6, 28, 6,6);
        g2.rotate(Math.toRadians(45), w/2, 60);
        g2.fillRoundRect(w/2-14, 56, 28, 6, 6,6);
        g2.fillRoundRect(w/2-3, 45, 6, 28, 6,6);
        g2.rotate(Math.toRadians(-45), w/2, 60);
      }

      // texto
      g2.setFont(Theme.H1); g2.setColor(ok? new Color(36,161,79): Theme.RED_DARK);
      drawCentered(g2, title, w, 115);
      g2.setFont(Theme.BODY); g2.setColor(Theme.TEXT_MAIN);
      drawCentered(g2, msg, w, 155);
      g2.dispose();
    }
    private void drawCentered(Graphics2D g2, String s, int w, int y){
      int sw=g2.getFontMetrics().stringWidth(s);
      g2.drawString(s, (w-sw)/2, y);
    }
  }
}
