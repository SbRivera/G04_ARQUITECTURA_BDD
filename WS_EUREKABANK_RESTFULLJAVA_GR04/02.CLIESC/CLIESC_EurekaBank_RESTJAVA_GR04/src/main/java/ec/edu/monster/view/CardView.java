package ec.edu.monster.view;

import javax.swing.*;
import java.awt.*;

public class CardView extends RoundedPanel {
  private final JLabel title = new JLabel("", SwingConstants.CENTER);
  private final JPanel header = new JPanel(){
    @Override protected void paintComponent(Graphics g){
      super.paintComponent(g);
      Graphics2D g2=(Graphics2D)g.create();
      g2.setRenderingHint(RenderingHints.KEY_ANTIALIASING, RenderingHints.VALUE_ANTIALIAS_ON);
      g2.setPaint(Theme.headerGradient(getWidth()));
      g2.fillRoundRect(0,0,getWidth(),getHeight()+20,20,20);
      // trama
      g2.setColor(new Color(255,255,255,35));
      for(int x=8;x<getWidth();x+=14) for(int y=8;y<getHeight();y+=14) g2.fillOval(x,y,2,2);
      g2.dispose();
    }
  };

  public CardView(String text, String emoji){
    super(22);
    setLayout(new BorderLayout());
    header.setOpaque(false); header.setPreferredSize(new Dimension(200,110));
    JLabel ico = new JLabel(emoji, SwingConstants.CENTER);
    ico.setFont(new Font("Segoe UI Emoji", Font.PLAIN, 48));
    ico.setForeground(Color.WHITE);
    header.setLayout(new BorderLayout());
    header.add(ico, BorderLayout.CENTER);
    add(header, BorderLayout.NORTH);

    title.setText(text);
    title.setFont(Theme.H2);
    title.setForeground(Theme.TEXT_MAIN);

    JPanel body = new JPanel(new BorderLayout());
    body.setOpaque(false);
    body.add(title, BorderLayout.CENTER);
    add(body, BorderLayout.CENTER);

    setCursor(Cursor.getPredefinedCursor(Cursor.HAND_CURSOR));
  }

  public void setContent(Component c){
    JPanel wrap = new JPanel(new BorderLayout());
    wrap.setOpaque(false);
    wrap.setBorder(BorderFactory.createEmptyBorder(8,18,18,18));
    wrap.add(c, BorderLayout.CENTER);
    add(wrap, BorderLayout.SOUTH);
    revalidate(); repaint();
  }
}
