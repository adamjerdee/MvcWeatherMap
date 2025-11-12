// Google Maps interop + NWS forecast loader
(() => {
    let resolveReady;
    const ready = new Promise(res => (resolveReady = res));

    // Defined before Google script loads
    window.initMap = () => resolveReady();

    async function ensureMapsReady() {
        await ready;
        if (!window.google || !google.maps) await new Promise(r => setTimeout(r, 0));
        const { Map } = await google.maps.importLibrary("maps");
        return { Map };
    }

    document.addEventListener("DOMContentLoaded", () => {
        createMap().catch(console.error);
    });

    async function createMap() {
        const { Map } = await ensureMapsReady();
        const el = document.getElementById("map");
        if (!el) return;

        const map = new Map(el, {
            center: { lat: 41.5868, lng: -93.6250 },
            zoom: 11,
            mapTypeControl: false,
            streetViewControl: false,
            fullscreenControl: false
        });

        map.addListener("click", async (e) => {
            const lat = e.latLng.lat();
            const lng = e.latLng.lng();
            await loadForecast(lat, lng);
        });
    }

    async function loadForecast(lat, lng) {
        const title = document.getElementById("forecastTitle");
        const cityState = document.getElementById("cityState");
        const topRow = document.getElementById("topRow");
        const bottomRow = document.getElementById("bottomRow");
        if (!topRow || !bottomRow) return;

        title.style.display = "block";
        cityState.textContent = " (loading…)";
        topRow.innerHTML = ""; bottomRow.innerHTML = "";

        try {
            const resp = await fetch(`/weather/forecast?lat=${lat}&lng=${lng}`);
            if (!resp.ok) throw new Error(`HTTP ${resp.status}`);
            const data = await resp.json();

            // Title city, state
            if (data.city) {
                cityState.textContent = ` (${data.city}${data.state ? ", " + data.state : ""})`;
            } else {
                cityState.textContent = "";
            }

            // Build two rows: top=current period type per date, bottom=opposite
            const periods = data.periods || [];
            const grouped = groupByDate(periods).slice(0, 7);
            const topIsDay = detectTopIsDay(periods);

            const topSlots = [];
            const bottomSlots = [];
            for (let i = 0; i < 7; i++) {
                const d = grouped[i];
                if (!d) { topSlots.push(null); bottomSlots.push(null); continue; }
                if (topIsDay) {
                    topSlots.push(d.day || d.night);
                    bottomSlots.push(d.night || d.day);
                } else {
                    topSlots.push(d.night || d.day);
                    bottomSlots.push(d.day || d.night);
                }
            }

            renderRow(topRow, topSlots);
            renderRow(bottomRow, bottomSlots);
        } catch (err) {
            cityState.textContent = " (failed to load)";
            console.error(err);
        }
    }

    function groupByDate(periods) {
        const map = new Map();
        for (const p of periods) {
            const dt = new Date(p.startTime);
            const key = dt.getFullYear() + "-" + (dt.getMonth() + 1) + "-" + dt.getDate();
            const entry = map.get(key) || { day: null, night: null };
            if (p.isDaytime) {
                if (!entry.day) entry.day = p;
            } else {
                if (!entry.night) entry.night = p;
            }
            map.set(key, entry);
        }
        return Array.from(map.entries())
            .sort((a, b) => new Date(a[0]) - new Date(b[0]))
            .map(x => x[1]);
    }

    function detectTopIsDay(periods) {
        if (!periods?.length) return true;
        const now = new Date();
        let current = null;
        for (const p of periods) {
            const st = new Date(p.startTime);
            if (st <= now && (!current || st > new Date(current.startTime))) {
                current = p;
            }
        }
        return current ? !!current.isDaytime : true;
    }

    function renderRow(container, slots) {
        container.innerHTML = "";
        for (const p of slots) {
            const cell = document.createElement("div");
            cell.className = "forecast-cell";
            if (!p) { cell.innerHTML = `<div class="muted">No data</div>`; container.appendChild(cell); continue; }
            cell.innerHTML = `
        <h6>${escapeHtml(p.name ?? "")}</h6>
        <div class="forecast-mini">
          ${p.icon ? `<img src="${p.icon}" alt="${escapeHtml(p.shortForecast ?? "")}"/>` : ""}
          <div>
            <div class="temp">${p.temperature == null ? "—" : `${p.temperature}${p.temperatureUnit ?? ""}`}</div>
            <div class="muted">${escapeHtml(p.shortForecast ?? "")}</div>
          </div>
        </div>`;
            container.appendChild(cell);
        }
    }

    function escapeHtml(s) { return (s || "").replace(/[&<>"']/g, c => ({ '&': '&amp;', '<': '&lt;', '>': '&gt;', '"': '&quot;', "'": '&#39;' }[c])); }
})();
