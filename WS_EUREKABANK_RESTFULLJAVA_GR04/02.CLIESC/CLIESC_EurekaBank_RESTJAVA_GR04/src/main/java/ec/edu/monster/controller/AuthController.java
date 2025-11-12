package ec.edu.monster.controller;

import ec.edu.monster.service.EurekaApi;
import ec.edu.monster.view.LoginView;
import ec.edu.monster.view.MenuView;

import javax.swing.*;

public class AuthController {
  private final LoginView view;
  private final EurekaApi api = new EurekaApi();

  public AuthController(LoginView view) {
    this.view = view;
    init();
  }

  private void init() {
    view.onLoginClick((u, p) -> {
      try {
        if (!api.ping()) {
          JOptionPane.showMessageDialog(view, "Servidor no disponible (ping falló).",
              "Conexión", JOptionPane.WARNING_MESSAGE);
          return;
        }
        boolean ok = api.login(u, p);
        if (ok) {
          view.dispose();
          MenuView menu = new MenuView(u);
          // Crea el controller (hace el wiring en el constructor)
          new OperacionesController(menu, api);
          // Muestra la vista sin llamar a mostrar()
          menu.setVisible(true);
        } else {
          JOptionPane.showMessageDialog(view, "Credenciales inválidas.", "Login",
              JOptionPane.ERROR_MESSAGE);
        }
      } catch (Exception ex) {
        JOptionPane.showMessageDialog(view, "Error de login: " + ex.getMessage(),
            "Login", JOptionPane.ERROR_MESSAGE);
      }
    });
  }

  public void mostrar() { view.setVisible(true); }
}
