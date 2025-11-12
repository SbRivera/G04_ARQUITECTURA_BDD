package ec.edu.monster.service;

import java.io.IOException;
import java.net.URI;
import java.net.URLEncoder;
import java.net.http.HttpClient;
import java.net.http.HttpRequest;
import java.net.http.HttpResponse;
import java.nio.charset.StandardCharsets;
import java.time.Duration;
import java.util.Map;
import java.util.StringJoiner;

public class ApiClient {

  private final HttpClient http = HttpClient.newBuilder()
      .connectTimeout(Duration.ofSeconds(5))
      .build();

  public String get(String url) throws IOException, InterruptedException {
    HttpRequest req = HttpRequest.newBuilder()
        .uri(URI.create(url))
        .timeout(Duration.ofSeconds(10))
        .GET()
        .build();
    HttpResponse<String> res = http.send(req, HttpResponse.BodyHandlers.ofString());
    if (res.statusCode() >= 200 && res.statusCode() < 300) return res.body();
    throw new IOException("GET " + url + " -> " + res.statusCode() + ": " + res.body());
  }

  public String post(String url, Map<String, String> queryParams)
      throws IOException, InterruptedException {

    String full = url + (queryParams == null || queryParams.isEmpty()
        ? "" : "?" + encode(queryParams));

    HttpRequest req = HttpRequest.newBuilder()
        .uri(URI.create(full))
        .timeout(Duration.ofSeconds(10))
        .header("Content-Type", "application/json")
        .POST(HttpRequest.BodyPublishers.noBody())
        .build();

    HttpResponse<String> res = http.send(req, HttpResponse.BodyHandlers.ofString());
    if (res.statusCode() >= 200 && res.statusCode() < 300) return res.body();
    throw new IOException("POST " + full + " -> " + res.statusCode() + ": " + res.body());
  }

  private static String encode(Map<String, String> params) {
    StringJoiner sj = new StringJoiner("&");
    for (var e : params.entrySet()) {
      sj.add(URLEncoder.encode(e.getKey(), StandardCharsets.UTF_8)
          + "=" + URLEncoder.encode(String.valueOf(e.getValue()), StandardCharsets.UTF_8));
    }
    return sj.toString();
  }
}
