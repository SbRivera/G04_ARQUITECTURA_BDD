/*
 * Click nbfs://nbhost/SystemFileSystem/Templates/Licenses/license-default.txt to change this license
 * Click nbfs://nbhost/SystemFileSystem/Templates/WebServices/WebService.java to edit this template
 */
package ec.edu.monster.ws;

import ec.edu.monster.modelo.Movimiento;
import ec.edu.monster.servicio.EurekaService;
import jakarta.jws.WebMethod;
import jakarta.jws.WebParam;
import jakarta.jws.WebResult;
import jakarta.jws.WebService;
import java.util.ArrayList;
import java.util.List;


/**
 *
 * @author JOIS
 */
@WebService(serviceName = "WSEureka")
public class WSEureka {

    //LOGIN
    
    
    @WebMethod(operationName = "validarIngreso")
    public String validarIngreso(@WebParam(name = "usuario") String usuario,@WebParam(name = "password") String password) {
        EurekaService usuarioServicio = new EurekaService();
        boolean exitoso = usuarioServicio.validarIngreso(usuario, password);
        if(exitoso){
            return "Exitoso";
        }
            return "Denegado";
    }
    
    
    /**
     * Web service operation
     * @param cuenta
     * @return Retorna la lista de movimientos de una cuenta
     */
    @WebMethod(operationName = "traerMovimientos")
    @WebResult(name = "movimiento")
    public List<Movimiento> traerMovimientos(@WebParam(name = "cuenta") String cuenta) {
        
        List<Movimiento> lista;
        
        //proceso
        try {
            //recuperar moviemintos
            EurekaService service =new EurekaService();
            lista = service.leerMovimientos(cuenta);
        } catch (Exception e) {
            //en caso de error, retorne una lista vacia
            lista = new ArrayList<>();
        }
        
        //retorno
        return lista;
    }

    /**
     * Web service operation
     * @param cuenta
     * @param importe
     * @return Estado, 1 o -1
     */
    @WebMethod(operationName = "regDeposito")
    @WebResult(name = "estado")
    public int regDeposito(@WebParam(name = "cuenta") String cuenta, @WebParam(name = "importe") double importe) {
        int estado;
        
        //proceso
        String codEmp = "0001";
        try {
            EurekaService service = new EurekaService();
            service.registrarDeposito(cuenta, importe, codEmp);
            estado = 1;
        } catch (Exception e) {
            estado = -1;
        }
        
        //retorno
        return estado;
    }
    
    @WebMethod(operationName = "regRetiro")
    @WebResult(name = "estado")
    public int regRetiro(@WebParam(name = "cuenta") String cuenta, @WebParam(name = "importe") double importe) {
        int estado;
        String codEmp = "0004";
        try {
            EurekaService service = new EurekaService();
            service.registrarRetiro(cuenta, importe, codEmp);
            estado = 1; // Éxito
            System.out.println("Retiro exitoso: cuenta=" + cuenta + ", importe=" + importe);
        } catch (Exception e) {
            estado = -1; // Error
            System.err.println("Error al realizar el retiro: " + e.getMessage());
        }
        return estado;
    }

    
    @WebMethod(operationName = "regTransferencia")
    @WebResult(name = "estado")
    public int regTransferencia(@WebParam(name = "cuentaOrigen") String cuentaOrigen, 
                                @WebParam(name = "cuentaDestino") String cuentaDestino, 
                                @WebParam(name = "importe") double importe) {
        int estado;
        String codEmp = "0004";
        try {
            EurekaService service = new EurekaService();
            service.registrarTransferencia(cuentaOrigen, cuentaDestino, importe, codEmp);
            estado = 1; // Éxito
        } catch (Exception e) {
            estado = -1; // Error
        }
        return estado;
    }

}
