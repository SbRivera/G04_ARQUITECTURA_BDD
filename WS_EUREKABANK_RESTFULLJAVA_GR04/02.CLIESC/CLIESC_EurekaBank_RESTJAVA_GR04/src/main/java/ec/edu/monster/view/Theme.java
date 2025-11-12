package ec.edu.monster.view;

import java.awt.*;

public final class Theme {
  private Theme(){}

  // Colores base (ajustados a tus mocks)
  public static final Color NAVY_DARK = new Color(10, 26, 45);
  public static final Color NAVY      = new Color(17, 42, 72);
  public static final Color RED       = new Color(200, 16, 30);
  public static final Color RED_DARK  = new Color(170, 12, 23);
  public static final Color BLUE      = new Color(33, 76, 170);
  public static final Color BLUE_DARK = new Color(28, 60, 130);
  public static final Color CARD_BG   = new Color(250, 250, 252);
  public static final Color CARD_SHADOW = new Color(0,0,0,40);
  public static final Color TEXT_MAIN = new Color(30, 30, 34);
  public static final Color TEXT_MUTED = new Color(120, 120, 130);

  public static final Font TITLE = new Font("Segoe UI", Font.BOLD, 22);
  public static final Font H1    = new Font("Segoe UI", Font.BOLD, 26);
  public static final Font H2    = new Font("Segoe UI", Font.BOLD, 18);
  public static final Font BODY  = new Font("Segoe UI", Font.PLAIN, 14);

  public static GradientPaint headerGradient(int w){
    return new GradientPaint(0,0, RED, w,0, BLUE);
  }
}
