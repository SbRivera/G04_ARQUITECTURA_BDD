package ec.edu.monster.controller;

import ec.edu.monster.service.EurekaApi;
import ec.edu.monster.view.*;

import javax.swing.*;
import java.util.Map;

public class OperacionesController {

  private final MenuView menu;
  private final EurekaApi api;

  public OperacionesController(MenuView menu, EurekaApi api) {
    this.menu = menu;
    this.api = api;
    wire();
  }

  private void wire() {
    menu.onDeposito(() -> {
      DepositoView d = new DepositoView(menu);
      d.onEjecutar((cta, imp) -> ejecutar(() -> api.deposito(cta, imp)));
      d.setVisible(true);
    });

    menu.onRetiro(() -> {
      RetiroView r = new RetiroView(menu);
      r.onEjecutar((cta, imp) -> ejecutar(() -> api.retiro(cta, imp)));
      r.setVisible(true);
    });

    menu.onTransferencia(() -> {
      TransferenciaView t = new TransferenciaView(menu);
      t.onEjecutar((ori, des, imp) -> ejecutarTrf(() -> api.transferencia(ori, des, imp)));
      t.setVisible(true);
    });

    menu.onMovimientos(() -> {
      MovimientosView mv = new MovimientosView(menu);
      mv.onBuscar(cta -> {
        try {
          mv.setCuentaBadge(cta.isEmpty() ? "—" : cta);
          mv.setTableModel(api.movimientos(cta));
        } catch (Exception ex) {
          Dialogs.error(menu, "Error cargando movimientos: " + ex.getMessage());
        }
      });
      // si escribió en la mini card del menú
      String ctaInicial = menu.getCuentaMov();
      if(!ctaInicial.isEmpty()){
        try { mv.setCuentaBadge(ctaInicial); mv.setTableModel(api.movimientos(ctaInicial)); }
        catch (Exception ex){ Dialogs.error(menu, "Error cargando movimientos: " + ex.getMessage()); }
      }
      mv.setVisible(true);
    });

    menu.onSalir(menu::dispose);
  }

  private void ejecutar(ApiCall call){
    try {
      Map<String,Object> res = call.run();
      if(((Number)res.get("estado")).intValue() == 1)
        Dialogs.success(menu, "Operación realizada correctamente. Saldo: " + res.get("saldo"));
      else
        Dialogs.error(menu, "No se pudo completar la operación.");
    } catch (Exception ex) {
      Dialogs.error(menu, ex.getMessage());
    }
  }

  private void ejecutarTrf(ApiCall call){
    try {
      Map<String,Object> res = call.run();
      if(((Number)res.get("estado")).intValue() == 1)
        Dialogs.success(menu, "Transferencia realizada. Saldo origen: " + res.get("saldo"));
      else
        Dialogs.error(menu, "No se pudo completar la transferencia.");
    } catch (Exception ex) {
      Dialogs.error(menu, ex.getMessage());
    }
  }

  @FunctionalInterface interface ApiCall { Map<String,Object> run() throws Exception; }
}
