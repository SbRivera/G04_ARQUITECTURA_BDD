package ec.edu.monster.ws;

import ec.edu.monster.modelo.Movimiento;
import ec.edu.monster.servicio.EurekaService;
import jakarta.jws.WebParam;

import jakarta.ws.rs.*;
import jakarta.ws.rs.core.MediaType;
import jakarta.ws.rs.core.Response;

import java.util.List;

@Path("corebancario")
public class CoreBancarioResource {

    private final EurekaService service = new EurekaService();

    /**
     * Web service operation to register a deposit.
     * @param cuenta the account number
     * @param importe the deposit amount
     * @return Response with status 1 (success) or -1 (failure)
     */
    @POST
    @Path("deposito")
    @Consumes(MediaType.APPLICATION_JSON)
    @Produces(MediaType.APPLICATION_JSON)
    public Response registrarDeposito(@QueryParam("cuenta") String cuenta,
                                      @QueryParam("importe") double importe) {
        int estado;
        
        // Asignar un código de empleado fijo
        String codEmp = "0001";

        try {
            // Llamar al servicio de depósito
            service.registrarDeposito(cuenta, importe, codEmp);
            estado = 1; // Éxito
        } catch (Exception e) {
            estado = -1; // Error
        }
        
        // Devolver el estado como respuesta JSON
        return Response.ok("{\"estado\": " + estado + "}").build();
    }
    
    @POST
    @Path("retiro")
    @Consumes(MediaType.APPLICATION_JSON)
    @Produces(MediaType.APPLICATION_JSON)
    public Response registrarRetiro(@QueryParam("cuenta") String cuenta,
                                     @QueryParam("importe") double importe) {
        int estado;
        String codEmp = "0001"; // Código fijo del empleado
        try {
            service.registrarRetiro(cuenta, importe, codEmp);
            estado = 1; // Éxito
        } catch (Exception e) {
            estado = -1; // Error
        }
        return Response.ok("{\"estado\": " + estado + "}").build();
    }

    @POST
    @Path("transferencia")
    @Consumes(MediaType.APPLICATION_JSON)
    @Produces(MediaType.APPLICATION_JSON)
    public Response registrarTransferencia(@QueryParam("cuentaOrigen") String cuentaOrigen,
                                            @QueryParam("cuentaDestino") String cuentaDestino,
                                            @QueryParam("importe") double importe) {
        int estado;
        String codEmp = "0001"; // Código fijo del empleado
        try {
            service.registrarTransferencia(cuentaOrigen, cuentaDestino, importe, codEmp);
            estado = 1; // Éxito
        } catch (Exception e) {
            estado = -1; // Error
        }
        return Response.ok("{\"estado\": " + estado + "}").build();
    }



    @GET
    @Path("movimientos/{cuenta}")
    @Produces(MediaType.APPLICATION_JSON)
    public Response obtenerMovimientos(@PathParam("cuenta") String cuenta) {
        try {
            List<Movimiento> movimientos = service.leerMovimientos(cuenta);
            return Response.ok(movimientos).build();
        } catch (Exception e) {
            return Response.status(Response.Status.INTERNAL_SERVER_ERROR)
                           .entity("{\"error\":\"" + e.getMessage() + "\"}").build();
        }
    }
    
    
}
