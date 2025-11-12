package ec.edu.monster.cliesc_eurekabank_restjava_gr04;

import ec.edu.monster.controller.AuthController;
import ec.edu.monster.view.LoginView;

public class App {
  public static void main(String[] args) {
    javax.swing.SwingUtilities.invokeLater(() -> {
      LoginView view = new LoginView();
      new AuthController(view).mostrar();
    });
  }
}
