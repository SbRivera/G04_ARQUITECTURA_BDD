/*
 * Click nbfs://nbhost/SystemFileSystem/Templates/Licenses/license-default.txt to change this license
 * Click nbfs://nbhost/SystemFileSystem/Templates/Classes/Class.java to edit this template
 */
package ec.edu.monster.controlador;

import ec.edu.monster.servicio.EurekaService;
import ec.edu.monster.ws.Movimiento;
import java.util.List;

/**
 *
 * @author JOIS
 */
public class CliCon_Controlador {
    
    public List<Movimiento> traerMovimientos(String cuenta){
        EurekaService service=new EurekaService();
       return service.traerMovimientos(cuenta);
           
    }
    
    public int regDeposito(String cuenta,double importe){
        EurekaService service=new EurekaService();
       return service.regDeposito(cuenta,importe);
           
    }
    
    public int regRetiro(String cuenta,double importe){
       EurekaService service=new EurekaService();
       return service.regRetiro(cuenta, importe);
    }
    
    public int regTransferencia(String cuentaOrigen, String cuentaDestino,double importe){
       EurekaService service=new EurekaService();
       return service.regTransferencia(cuentaOrigen, cuentaDestino, importe);
    }

    
    /*
    public String validarIngreso (String usuario, String password){
        EurekaService service=new EurekaService();
        return service.validarIngreso(usuario, password);
    }
    */
}