document.addEventListener("DOMContentLoaded", function () {
    const root = document.documentElement;
    const overlay = document.getElementById("sidebarOverlay");
    const desktopToggle = document.getElementById("desktopSidebarToggle");
    const mobileToggle = document.getElementById("mobileSidebarToggle");
    const mobileClose = document.getElementById("mobileSidebarClose");

    const desktopKey = "hospivital-sidebar-collapsed";
    const mobileKey = "hospivital-sidebar-open-mobile";

    function isMobile() {
        return window.innerWidth <= 992;
    }

    function applyInitialState() {
        if (isMobile()) {
            root.classList.remove("sidebar-collapsed");

            const mobileSaved = localStorage.getItem(mobileKey);
            if (mobileSaved === "true") {
                root.classList.add("sidebar-open");
            } else {
                root.classList.remove("sidebar-open");
            }
        } else {
            root.classList.remove("sidebar-open");

            const desktopSaved = localStorage.getItem(desktopKey);
            if (desktopSaved === "true") {
                root.classList.add("sidebar-collapsed");
            } else {
                root.classList.remove("sidebar-collapsed");
            }
        }
    }

    function toggleDesktopSidebar() {
        root.classList.toggle("sidebar-collapsed");
        localStorage.setItem(
            desktopKey,
            root.classList.contains("sidebar-collapsed")
        );
    }

    function openMobileSidebar() {
        root.classList.add("sidebar-open");
        localStorage.setItem(mobileKey, "true");
    }

    function closeMobileSidebar() {
        root.classList.remove("sidebar-open");
        localStorage.setItem(mobileKey, "false");
    }

    applyInitialState();

    desktopToggle?.addEventListener("click", function () {
        if (!isMobile()) {
            toggleDesktopSidebar();
        }
    });

    mobileToggle?.addEventListener("click", function () {
        if (isMobile()) {
            openMobileSidebar();
        } else {
            toggleDesktopSidebar();
        }
    });

    mobileClose?.addEventListener("click", function () {
        if (isMobile()) {
            closeMobileSidebar();
        }
    });

    overlay?.addEventListener("click", function () {
        if (isMobile()) {
            closeMobileSidebar();
        }
    });

    window.addEventListener("resize", applyInitialState);
});