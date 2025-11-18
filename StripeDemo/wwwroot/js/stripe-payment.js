function initializeStripePayment(publicKey, amount) {
    // ÉTAPE 0. Initialiser et configurer Stripe.js
    const stripe = Stripe(publicKey);
    const elements = stripe.elements();

    // Créer et monter l'élément de carte de crédit dans le formulaire
    const card = elements.create("card", { hidePostalCode: true });
    card.mount("#credit-card");

    // Afficher les erreurs de validation de carte en temps réel
    card.on("change", event => {
        const errorDiv = document.getElementById("card-errors");
        errorDiv.textContent = event.error ? event.error.message : "";
    });

    // Gérer la soumission du formulaire de paiement
    document.getElementById("payment-form").addEventListener("submit", async (event) => {
        event.preventDefault();

        const customerName = document.getElementById("name").value;
        const customerEmail = document.getElementById("email").value;

        try {
            // ÉTAPE 1 : Créer un PaymentIntent côté serveur
            // Le serveur crée un PaymentIntent auprès de Stripe.Net et retourne
            // un clientSecret qui permettra de confirmer le paiement côté client
            const { data } = await axios.post("/transaction/create-payment-intent", {
                customerName: customerName,
                customerEmail: customerEmail,
                amount: amount
            });

            const { transactionId, clientSecret } = data;

            // ÉTAPE 2 : Confirmer le paiement avec Stripe.js (côté client)
            // Stripe traite le paiement de manière sécurisée avec les
            // informations de carte saisies par l'utilisateur
            const result = await stripe.confirmCardPayment(clientSecret, {
                payment_method: { card: card }
            });

            // Vérifier si une erreur s'est produite lors de la confirmation
            if (result.error) {
                const errorDiv = document.getElementById("card-errors");
                errorDiv.textContent = result.error.message;
                return;
            }

            // ÉTAPE 3 : Vérifier le paiement côté serveur
            // Le serveur récupère le statut final du PaymentIntent depuis
            // Stripe.Net et met à jour la transaction en base de données
            await axios.post("/transaction/verify-payment", {
                transactionId: transactionId,
                paymentIntentId: result.paymentIntent.id
            });

            // ÉTAPE 4 : Rediriger vers la page de confirmation (Succès)
            window.location.href = "/transaction/confirmation/" + transactionId;

        } catch (error) {
            // Afficher un message d'erreur générique en cas de problème
            const errorDiv = document.getElementById("card-errors");
            errorDiv.textContent = error.message;
        }
    });
}
