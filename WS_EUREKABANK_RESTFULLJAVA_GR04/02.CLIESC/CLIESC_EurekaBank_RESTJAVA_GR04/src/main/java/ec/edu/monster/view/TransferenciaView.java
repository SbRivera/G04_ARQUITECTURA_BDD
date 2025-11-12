package ec.edu.monster.view;

import javax.swing.*;
import java.awt.*;

public class TransferenciaView extends JDialog {
  private final RoundTextField txtOri = new RoundTextField(18);
  private final RoundTextField txtDes = new RoundTextField(18);
  private final JSpinner spImporte = new JSpinner(
      new SpinnerNumberModel(0.00, 0.01, 1_000_000.0, 1.0));
  private final PrimaryButton btn = new PrimaryButton("TRANSFERIR");

  public TransferenciaView(Window owner){ // <- importante: recibe owner
    super(owner, "Transferencia", ModalityType.APPLICATION_MODAL);
    setContentPane(new GradientBackgroundPanel());
    setLayout(new GridBagLayout());
    RoundedPanel card = new RoundedPanel(24);
    card.setLayout(new GridBagLayout());
    GridBagConstraints c = new GridBagConstraints();
    c.insets=new Insets(8,12,8,12); c.gridx=0; c.gridy=0; c.gridwidth=2;
    JLabel title = new JLabel("Transferencia"); title.setFont(Theme.H1);
    card.add(title,c);

    c.gridy=1; c.gridwidth=1; card.add(new JLabel("Cuenta origen:"), c);
    c.gridx=1; txtOri.setPlaceholder("Cuenta origen"); card.add(txtOri, c);

    c.gridx=0; c.gridy=2; card.add(new JLabel("Cuenta destino:"), c);
    c.gridx=1; txtDes.setPlaceholder("Cuenta destino"); card.add(txtDes, c);

    c.gridx=0; c.gridy=3; card.add(new JLabel("Importe:"), c);
    c.gridx=1; ((JSpinner.DefaultEditor)spImporte.getEditor()).getTextField().setColumns(14);
    card.add(spImporte, c);

    c.gridx=0; c.gridy=4; c.gridwidth=2; c.insets=new Insets(16,12,12,12);
    card.add(btn, c);

    add(card, new GridBagConstraints());
    setSize(540, 420); setLocationRelativeTo(owner);
  }

  public void onEjecutar(TriConsumer<String,String,Double> h){
    btn.addActionListener(e -> {
      h.accept(txtOri.getText().trim(), txtDes.getText().trim(),
          ((Number)spImporte.getValue()).doubleValue());
      dispose();
    });
  }

  // TriConsumer propio (no de java.util.function)
  @FunctionalInterface public interface TriConsumer<A,B,C> { void accept(A a, B b, C c); }
}
