package ec.edu.monster.view;

import javax.swing.*;
import java.awt.*;
import java.util.function.BiConsumer;

public class RetiroView extends JDialog {
  private final RoundTextField txtCuenta = new RoundTextField(18);
  private final JSpinner spImporte = new JSpinner(new SpinnerNumberModel(0.00,0.01,1_000_000.0,1.0));
  private final PrimaryButton btn = new PrimaryButton("RETIRAR");

  public RetiroView(Window owner){
    super(owner, "Retiro", ModalityType.APPLICATION_MODAL);
    setContentPane(new GradientBackgroundPanel());
    setLayout(new GridBagLayout());
    RoundedPanel card = new RoundedPanel(24);
    card.setLayout(new GridBagLayout());
    GridBagConstraints c = new GridBagConstraints();
    c.insets=new Insets(8,12,8,12); c.gridx=0; c.gridy=0; c.gridwidth=2;
    JLabel title = new JLabel("Retiro"); title.setFont(Theme.H1); title.setForeground(Theme.TEXT_MAIN);
    card.add(title,c);

    c.gridy=1; c.gridwidth=1; card.add(new JLabel("Cuenta:"), c);
    c.gridx=1; txtCuenta.setPlaceholder("Número de cuenta"); card.add(txtCuenta, c);

    c.gridx=0; c.gridy=2; card.add(new JLabel("Importe:"), c);
    c.gridx=1; ((JSpinner.DefaultEditor)spImporte.getEditor()).getTextField().setColumns(14);
    card.add(spImporte, c);

    c.gridx=0; c.gridy=3; c.gridwidth=2; c.insets=new Insets(16,12,12,12);
    card.add(btn, c);

    add(card, new GridBagConstraints());
    setSize(520, 360); setLocationRelativeTo(owner);
  }

  public void onEjecutar(BiConsumer<String, Double> h){
    btn.addActionListener(e -> {
      String cta = txtCuenta.getText().trim();
      double imp = ((Number)spImporte.getValue()).doubleValue();
      h.accept(cta, imp);
      dispose();
    });
  }
}
