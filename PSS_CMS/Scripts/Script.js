function toggleMenuIcons() {

    const openfolds = document.querySelectorAll(".nav-link.openfold");

    openfolds.forEach(menu => {

        const settingsIcon = menu.querySelector(".settingsIcon");
        const openIcon = menu.querySelector(".openIcon");

        menu.addEventListener("click", function () {

            const isOpen = menu.classList.contains("active");

            // Close all
            openfolds.forEach(m => {
                m.classList.remove("active");
                m.querySelector(".settingsIcon").style.display = "inline";
                m.querySelector(".openIcon").style.display = "none";
            });

            // Open only if closed
            if (!isOpen) {
                menu.classList.add("active");
                settingsIcon.style.display = "none";
                openIcon.style.display = "inline";
            }
        });
    });
}



function setupEvents() {

    const sidebar = document.querySelector("nav");
    const toggle = document.querySelector(".toggle");

    // MAIN MENUS
    const configMenu = document.querySelector(".kb-menu");
    const configReport = document.querySelector(".report-menu");
    const configMenu1 = document.querySelector(".security-menu");

    // SUBMENUS
    const configItems = document.querySelectorAll(".kb-item");
    const configReportItems = document.querySelectorAll(".report-item");
    const configItems1 = document.querySelectorAll(".security-item");

    if (sidebar.classList.contains("close")) {
        document.querySelectorAll(".kb-item, .report-item, .security-item")
            .forEach(i => i.style.display = "none");
    }


    function closeAllSubmenus() {
        [...configItems, ...configReportItems, ...configItems1].forEach(i => {
            i.style.display = "none";
        });
    }

    function toggleSubmenu(items) {
        const isOpen = Array.from(items).some(i => i.style.display === "block");
        closeAllSubmenus();
        items.forEach(i => i.style.display = isOpen ? "none" : "block");
    }

    configMenu?.addEventListener("click", e => {
        e.stopPropagation();
        toggleSubmenu(configItems);
    });

    configReport?.addEventListener("click", e => {
        e.stopPropagation();
        toggleSubmenu(configReportItems);
    });

    configMenu1?.addEventListener("click", e => {
        e.stopPropagation();
        toggleSubmenu(configItems1);
    });

    toggle?.addEventListener("click", () => {
        sidebar.classList.toggle("close");
        closeAllSubmenus();
    });




    /* ACTIVE PAGE AUTO OPEN */
    document.querySelectorAll(".menu-links .nav-link a").forEach(link => {
        if (link.href === window.location.href) {

            const item = link.closest(".nav-link");
            item.style.display = "block";
            item.classList.add("active");

            if (item.classList.contains("kb-item")) configMenu.click();
            if (item.classList.contains("report-item")) configReport.click();
            if (item.classList.contains("security-item")) configMenu1.click();
        }
    });

   
}


// Update sidebar state by enabling/disabling pointer events
function updateSidebarState(sidebar) {
    sidebar.style.pointerEvents = sidebar.classList.contains("close") ? 'none' : 'auto';
}

window.onload = function () {
    setupEvents();
    toggleMenuIcons();
};
