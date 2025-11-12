package ec.edu.monster.service;

import com.google.gson.*;
import javax.swing.table.DefaultTableModel;
import java.io.IOException;
import java.util.LinkedHashMap;
import java.util.Map;

public class EurekaApi {

  // Ajusta tu context root aquí:
  public static String BASE_URL =
      "http://localhost:8080/WSEurekaBank_Restfull_Java_G4-1.0-SNAPSHOT/resources/corebancario";

  private final ApiClient http = new ApiClient();
  private final Gson gson = new GsonBuilder().setLenient().create();

  public boolean ping() {
    try { http.get(BASE_URL + "/ping"); return true; }
    catch (Exception e) { return false; }
  }

  public boolean login(String usuario, String password)
      throws IOException, InterruptedException {
    String body = http.post(BASE_URL + "/login",
        Map.of("usuario", usuario, "password", password));
    JsonObject json = JsonParser.parseString(body).getAsJsonObject();
    return "Exitoso".equalsIgnoreCase(json.get("resultado").getAsString());
  }

  public Map<String,Object> deposito(String cuenta, double importe)
      throws IOException, InterruptedException {
    String body = http.post(BASE_URL + "/deposito",
        Map.of("cuenta", cuenta, "importe", String.valueOf(importe)));
    return parseOperacion(body);
  }

  public Map<String,Object> retiro(String cuenta, double importe)
      throws IOException, InterruptedException {
    String body = http.post(BASE_URL + "/retiro",
        Map.of("cuenta", cuenta, "importe", String.valueOf(importe)));
    return parseOperacion(body);
  }

  public Map<String,Object> transferencia(String origen, String destino, double importe)
      throws IOException, InterruptedException {
    String body = http.post(BASE_URL + "/transferencia",
        Map.of("cuentaOrigen", origen, "cuentaDestino", destino,
               "importe", String.valueOf(importe)));
    return parseOperacion(body);
  }

  public DefaultTableModel movimientos(String cuenta)
      throws IOException, InterruptedException {
    String body = http.get(BASE_URL + "/movimientos/" + cuenta);
    JsonArray arr = JsonParser.parseString(body).getAsJsonArray();

    String[] cols = {"Nro", "Fecha", "Tipo", "Acción", "Importe"};
    DefaultTableModel model = new DefaultTableModel(cols, 0) {
      @Override public boolean isCellEditable(int r, int c) { return false; }
    };

    for (JsonElement el : arr) {
      JsonObject o = el.getAsJsonObject();
      Object nro     = o.has("nromov") ? o.get("nromov").getAsInt() : "";
      String fecha   = o.has("fechaAsString") ? o.get("fechaAsString").getAsString()
                     : (o.has("fecha") ? o.get("fecha").getAsString() : "");
      String tipo    = o.has("tipo") ? o.get("tipo").getAsString() : "";
      String accion  = o.has("accion") ? o.get("accion").getAsString() : "";
      double importe = o.has("importe") ? o.get("importe").getAsDouble() : 0.0;
