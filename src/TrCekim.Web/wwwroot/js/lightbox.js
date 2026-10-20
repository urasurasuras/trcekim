// Minimal lightbox for gallery pages. Uses event delegation, so it keeps working
// after Blazor enhanced navigation swaps the page content.
// Markup: <a data-lightbox="group" href="large.jpg" data-caption="…" data-order="/order-url">
(() => {
    let items = [];
    let index = 0;
    let box;

    function build() {
        box = document.createElement("div");
        box.className = "lightbox";
        box.setAttribute("role", "dialog");
        box.setAttribute("aria-modal", "true");
        box.innerHTML =
            '<button class="lb-close" aria-label="Kapat">✕</button>' +
            '<button class="lb-prev" aria-label="Önceki">‹</button>' +
            '<img alt="">' +
            '<button class="lb-next" aria-label="Sonraki">›</button>' +
            '<div class="lb-bar"><span class="lb-caption"></span><a class="btn lb-order" href="#">Sipariş ver</a></div>';
        document.body.appendChild(box);
        box.querySelector(".lb-close").addEventListener("click", close);
        box.querySelector(".lb-prev").addEventListener("click", () => show(index - 1));
        box.querySelector(".lb-next").addEventListener("click", () => show(index + 1));
        box.addEventListener("click", e => { if (e.target === box) close(); });
    }

    function show(i) {
        index = (i + items.length) % items.length;
        const a = items[index];
        box.querySelector("img").src = a.href;
        box.querySelector(".lb-caption").textContent = a.dataset.caption || "";
        const order = box.querySelector(".lb-order");
        order.hidden = !a.dataset.order;
        if (a.dataset.order) order.href = a.dataset.order;
    }

    function close() {
        box.classList.remove("open");
        box.querySelector("img").removeAttribute("src");
    }

    document.addEventListener("click", e => {
        const a = e.target.closest("a[data-lightbox]");
        if (!a || e.ctrlKey || e.metaKey || e.shiftKey) return;
        e.preventDefault();
        if (!box) build();
        items = [...document.querySelectorAll(`a[data-lightbox="${a.dataset.lightbox}"]`)];
        show(items.indexOf(a));
        box.classList.add("open");
    });

    document.addEventListener("keydown", e => {
        if (!box || !box.classList.contains("open")) return;
        if (e.key === "Escape") close();
        if (e.key === "ArrowLeft") show(index - 1);
        if (e.key === "ArrowRight") show(index + 1);
    });
})();
