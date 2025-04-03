function toggleMenuIcons() {
    // Get all elements with the class 'nav-link openfold'
    const toggleElements = document.querySelectorAll(".nav-link.openfold");

    toggleElements.forEach((element) => {
        const settingsIcon = element.querySelector(".settingsIcon");
        const openIcon = element.querySelector(".openIcon");

        // Add click event listener to each element with the 'nav-link openfold' class
        element.addEventListener("click", function () {
            // First, close all other menu items' icons
            toggleElements.forEach((el) => {
                const otherSettingsIcon = el.querySelector(".settingsIcon");
                const otherOpenIcon = el.querySelector(".openIcon");

                // Close all other items except the current one
                if (el !== element) {
                    otherSettingsIcon.style.display = "inline"; // Show closefolder.png
                    otherOpenIcon.style.display = "none"; // Hide open.png
                }
            });

            // Toggle visibility of icons for the clicked element
            if (openIcon.style.display === "none") {
                settingsIcon.style.display = "none";
                openIcon.style.display = "inline"; // Show open.png
            } else {
                settingsIcon.style.display = "inline"; // Show closefolder.png
                openIcon.style.display = "none"; // Hide open.png
            }
        });
    });

    // Handle the bx-chevron-right toggle icon click
    const toggleIcons = document.querySelectorAll(".bx-chevron-right");

    toggleIcons.forEach((toggleIcon) => {
        toggleIcon.addEventListener("click", function () {
            // Reset the state for the openfold items (close icons and show open icons)
            toggleElements.forEach((el) => {
                const settingsIcon = el.querySelector(".settingsIcon");
                const openIcon = el.querySelector(".openIcon");

                settingsIcon.style.display = "inline"; // Show closefolder.png
                openIcon.style.display = "none"; // Hide open.png
            });
        });
    });
}




function setupEvents() {
    const body = document.querySelector('body');
    const sidebar = body.querySelector('nav');
    const toggle = body.querySelector(".toggle");
    const searchBtn = body.querySelector(".search-box");
    const modeSwitch = body.querySelector(".toggle-switch");
    const modeText = body.querySelector(".mode-text");

    // Configuration menu and submenu items
    const configMenu = document.querySelector('.menu-links .nav-link:nth-child(1)');
    const configItems = document.querySelectorAll('.menu-links .nav-link:nth-child(2), .menu-links .nav-link:nth-child(3)');

    const configMenu1 = document.querySelector('.menu-links .nav-link:nth-child(4)');
    const configItems1 = document.querySelectorAll('.menu-links .nav-link:nth-child(5), .menu-links .nav-link:nth-child(6)');

    // Settings menu and submenu items
    //const settingsMenu = document.querySelector('.menu-links .nav-link:nth-child(12)');
    //const settingsItems = document.querySelectorAll('.menu-links .nav-link:nth-child(13), .menu-links .nav-link:nth-child(14), .menu-links .nav-link:nth-child(15)');


    //const settingsMenu2 = document.querySelector('.menu-links .nav-link:nth-child(16)');
    //const settingsItems2 = document.querySelectorAll('.menu-links .nav-link:nth-child(17), .menu-links .nav-link:nth-child(18), .menu-links .nav-link:nth-child(19)');

    // Initially hide all submenus, including the 5s Reports menu
    closeAllSubmenus();

    // Function to ensure only main menus are visible when the sidebar is opened
    function showMainMenusOnly() {
        configMenu.style.display = 'block';
        configMenu1.style.display = 'block';
        //settingsMenu.style.display = 'block';
        //settingsMenu2.style.display = 'block';

        // Hide all submenus when showing main menus
        closeAllSubmenus();
    }

    //// Hide Configuration and Settings items initially
    //configItems.forEach(item => item.style.display = 'none');
    //configItems1.forEach(item => item.style.display = 'none');
    //settingsItems.forEach(item => item.style.display = 'none');
    //settingsItems2.forEach(item => item.style.display = 'none');


    // Toggle sidebar visibility
    if (toggle) {
        toggle.addEventListener("click", (event) => {
            event.stopPropagation();
            sidebar.classList.toggle("close");
            updateSidebarState(sidebar);

            // Show only main menus when the sidebar is opened
            if (!sidebar.classList.contains("close")) {
                showMainMenusOnly();
            } else {
                closeAllSubmenus();
            }
        });
    }
    // Search button toggle sidebar
    if (searchBtn) {
        searchBtn.addEventListener("click", () => {
            if (!sidebar.classList.contains("close")) {
                sidebar.classList.add("close");
                updateSidebarState(sidebar);
                closeAllSubmenus();
            }
        });
    }

    // Toggle dark/light mode
    if (modeSwitch && modeText) {
        modeSwitch.addEventListener("click", () => {
            body.classList.toggle("dark");
            modeText.innerText = body.classList.contains("dark") ? "Light mode" : "Dark mode";
        });
    }
    // Function to toggle submenu items and close others
    function toggleSubmenu(items, keepVisible = []) {
        const isSubmenuOpen = Array.from(items).some(item => item.style.display === 'block');

        // Close all other submenus except the ones specified to keep visible
        closeAllSubmenus(keepVisible);

        // Toggle current submenu
        if (!isSubmenuOpen) {
            items.forEach(item => item.style.display = 'block');
        } else {
            items.forEach(item => item.style.display = 'none');
        }
    }

    // Close all submenus except specified ones
    function closeAllSubmenus(exceptions = []) {
        // Convert exceptions to an array of elements
        const exceptionSet = new Set(exceptions);
        const closeItem = (item) => {
            if (!exceptionSet.has(item)) item.style.display = 'none';
        };

        configItems.forEach(closeItem);
        configItems1.forEach(closeItem);
        //settingsItems.forEach(closeItem);
        //settingsItems2.forEach(closeItem);


    }


    // Toggle Configuration items visibility
    if (configMenu) {
        configMenu.addEventListener("click", (event) => {
            event.stopPropagation();
            toggleSubmenu(configItems);
        });

    }

    if (configMenu1) {
        configMenu1.addEventListener("click", (event) => {
            event.stopPropagation();
            toggleSubmenu(configItems1);
        });
    }

    // Toggle Settings items visibility
    //if (settingsMenu) {
    //    settingsMenu.addEventListener("click", (event) => {
    //        event.stopPropagation();
    //        toggleSubmenu(settingsItems);
    //    });
    //}

    //if (settingsMenu2) {
    //    settingsMenu2.addEventListener("click", (event) => {
    //        event.stopPropagation();
    //        toggleSubmenu(settingsItems2);
    //    });
    //}



}
// Update sidebar state by enabling/disabling pointer events
function updateSidebarState(sidebar) {
    sidebar.style.pointerEvents = sidebar.classList.contains("close") ? 'none' : 'auto';
}

window.onload = function () {
    setupEvents();
    toggleMenuIcons();
};
