(() => {
    const modalElement = document.getElementById("loginModal");
    if (!modalElement) return;

    const form = document.getElementById("login-form");
    const errorBox = document.getElementById("login-error");
    const returnUrlInput = document.getElementById("login-return-url");
    const modal = bootstrap.Modal.getOrCreateInstance(modalElement);

    const showError = (message) => {
        errorBox.textContent = message;
        errorBox.classList.remove("d-none");
    };

    const hideError = () => {
        errorBox.textContent = "";
        errorBox.classList.add("d-none");
    };

    const openLoginModal = () => {
        hideError();
        returnUrlInput.value = window.location.pathname + window.location.search;
        modal.show();
    };

    document.querySelectorAll("[data-login-modal]").forEach((trigger) => {
        trigger.addEventListener("click", (event) => {
            event.preventDefault();
            openLoginModal();
        });
    });

    modalElement.addEventListener("hidden.bs.modal", () => {
        form.reset();
        hideError();
    });

    form.addEventListener("submit", async (event) => {
        event.preventDefault();
        hideError();

        const response = await fetch("/account/login", {
            method: "POST",
            headers: {
                "X-Requested-With": "XMLHttpRequest",
                "RequestVerificationToken": window.getRequestVerificationToken(),
            },
            body: new FormData(form),
        });

        if (response.ok) {
            const data = await response.json();
            window.location.href = data.redirectUrl || "/";
            return;
        }

        let message = "Invalid login attempt.";
        try {
            const data = await response.json();
            if (data.error) message = data.error;
        } catch {
            // keep default message
        }

        showError(message);
    });

    const params = new URLSearchParams(window.location.search);
    if (params.get("showLogin") === "true") {
        params.delete("showLogin");
        const cleanUrl = params.toString()
            ? `${window.location.pathname}?${params}`
            : window.location.pathname;
        history.replaceState({}, "", cleanUrl);
        openLoginModal();
    }
})();
