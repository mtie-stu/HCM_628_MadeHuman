// printHelper80.js
export const PrintHelper80 = (() => {
    const CSS = `
@page { size: 80mm 80mm; margin: 0; }

.page {
  width: 80mm; height: 80mm;
  box-sizing: border-box;
  page-break-after: always;
  display:flex; align-items:center; justify-content:center;
  position:relative; padding:0.2mm; font-family: Arial, sans-serif;
}
.qr { width:58mm; height:58mm; display:flex; align-items:center; justify-content:center; }
.qr img { width:100%; height:100%; object-fit: contain; }
.brand { position:absolute; right:1.2mm; bottom:1.2mm; font-size:9pt; }

/* CHỈ in #print-root-80, ẩn mọi thứ khác */
@media print {
  html, body { margin:0; padding:0; -webkit-print-color-adjust: exact; print-color-adjust: exact; }
  body > * { display: none !important; }                    /* ẩn toàn bộ app */
  #print-root-80, #print-root-80 * { display: revert !important; visibility: visible !important; }
  /* giữ container “bình thường” khi in để page-break hoạt động */
  #print-root-80 { position: static !important; width: auto !important; height: auto !important; overflow: visible !important; }
}

`;

    function extractBody(html) {
        try {
            const doc = new DOMParser().parseFromString(html, "text/html");
            return doc.body ? doc.body.innerHTML : html;
        } catch {
            const m = html.match(/<body[^>]*>([\s\S]*?)<\/body>/i);
            return m ? m[1] : html;
        }
    }

    function buildMergedHTML(items) {
        const pages = items.map(x =>
            `<section class="page" data-id="${x.outboundTaskItemId || ""}">${extractBody(x.htmlContent)}</section>`
        ).join("");
        return `<!doctype html><html><head><meta charset="utf-8"><title>Print 80x80</title>
<style>${CSS}</style></head><body>${pages}</body></html>`;
    }

    function waitForImages(docOrRoot) {
        const imgs = docOrRoot.images ? Array.from(docOrRoot.images) : Array.from(docOrRoot.querySelectorAll("img"));
        if (imgs.length === 0) return Promise.resolve();
        return Promise.all(imgs.map(img => {
            if (img.complete && img.naturalWidth > 0) return;
            return new Promise(res => { img.onload = img.onerror = () => res(); });
        }));
    }

    async function printInPlace(items) {
        // in tại chỗ, không popup
        const rootId = "print-root-80";
        const root = document.getElementById(rootId) || (() => {
            const d = document.createElement("div");
            d.id = rootId;
            d.style.position = "fixed";
            d.style.left = "0"; d.style.top = "0";
            d.style.width = "0"; d.style.height = "0";
            d.style.overflow = "hidden"; d.style.zIndex = "-1";
            document.body.appendChild(d);
            return d;
        })();

        // inject 1 lần CSS
        if (!document.getElementById("print-style-80")) {
            const style = document.createElement("style");
            style.id = "print-style-80";
            style.textContent = CSS;
            document.head.appendChild(style);
        }

        // render pages
        root.innerHTML = "";
        items.forEach(x => {
            const s = document.createElement("section");
            s.className = "page";
            s.setAttribute("data-id", x.outboundTaskItemId || "");
            s.innerHTML = extractBody(x.htmlContent);
            root.appendChild(s);
        });

        await waitForImages(root);
        window.print();
        setTimeout(() => { root.innerHTML = ""; }, 600);
    }

    async function printViaPopup(items) {
        // 1 popup, 1 lần in
        const html = buildMergedHTML(items);
        const w = window.open("", "_blank", "width=900,height=900");
        if (!w) {
            // nếu popup bị chặn thì in tại chỗ
            return printInPlace(items);
        }
        w.document.open(); w.document.write(html); w.document.close();
        await waitForImages(w.document);
        w.focus(); w.print();
        setTimeout(() => w.close(), 600);
    }

    // public API
    return {
        /**
         * items: [{ outboundTaskItemId, htmlContent }, ...]
         * options: { mode: "popup" | "inplace" }  (default: "popup")
         */
        async print(items, options = { mode: "popup" }) {
            if (!Array.isArray(items) || items.length === 0) return;
            if (options.mode === "inplace") return printInPlace(items);
            return printViaPopup(items);
        }
    };
})();
