// Definiere die Funktion initializeFilePaste
function initializeFilePaste(fileDropContainer, inputFile) {
    fileDropContainer.addEventListener('paste', onPaste);

    function onPaste(event) {
        inputFile.files = event.clipboardData.files;
        const changeEvent = new Event('change', { bubbles: true });
        inputFile.dispatchEvent(changeEvent);
    }

    return {
        dispose: function () {
            fileDropContainer.removeEventListener('paste', onPaste);
        }
    };
}

// Definiere die Funktion getClipboardImage
async function getClipboardImage(dotNetHelper) {
    try {
        const clipboardItems = await navigator.clipboard.read();

        for (const item of clipboardItems) {
            for (const type of item.types) {
                if (type.startsWith("image/")) {
                    const blob = await item.getType(type);
                    const reader = new FileReader();

                    reader.onload = function (event) {
                        const base64String = event.target.result.split(',')[1];
                        dotNetHelper.invokeMethodAsync('OnImagePasted', base64String);
                    };

                    reader.readAsDataURL(blob);
                    return;
                }
            }
        }
    } catch (err) {
        console.error("Clipboard access error:", err);
    }
}

// Definiere validateFileSize als normale Funktion
function validateFileSize(file, maxSize) {
    if (file.size > maxSize) {
        return false; // Dateigröße überschreitet das Limit
    }
    return true; // Dateigröße ist innerhalb des Limits
}

// Mache die Funktionen global verfügbar, indem du sie an das window-Objekt anhängst
window.initializeFilePaste = initializeFilePaste;
window.getClipboardImage = getClipboardImage;
window.validateFileSize = validateFileSize;

window.addEventListener('scroll', function () {
    const shrinkingDiv = document.getElementById('shrinkingDiv');
    const content = document.getElementById('content');
    const scrollTop = window.pageYOffset || document.documentElement.scrollTop;

    if (scrollTop > 100) {
        shrinkingDiv.classList.add('scrolled');
        content.classList.add('scrolled');
    } else {
        shrinkingDiv.classList.remove('scrolled');
        content.classList.remove('scrolled');
    }
});
/*-----Scroll Cards-----*/
window.initScrollCardsLimited = (containerId, totalCards, maxVisibleCards, radius, dotNetRef) => {
    const container = document.getElementById(containerId);
    if (!container) return;

    const cardList = container.querySelector('.card-list');
    const cards = cardList.querySelectorAll('li');
    const progressBar = container.querySelector('.progress-bar');
    const scrollArea = container.querySelector('.scroll-area');

    console.log(`Initialisiere mit ${totalCards} Karten, ${maxVisibleCards} sichtbare Positionen, Radius: ${radius}`);

    // Speichere die ursprünglichen Karten-Daten für Rotation
    let currentRotationOffset = 0;
    let lastStartIndex = 0;

    function updateCardsPosition() {
        const scrollY = scrollArea.scrollTop;
        const maxScroll = scrollArea.scrollHeight - scrollArea.clientHeight;
        const scrollProgress = maxScroll > 0 ? Math.min(scrollY / maxScroll, 1) : 0;

        // Progress Bar aktualisieren
        progressBar.style.transform = `scaleX(${scrollProgress})`;

        // Berechne aktuellen Start-Index
        const currentStartIndex = Math.floor(scrollProgress * (totalCards - maxVisibleCards));

        // Berechne wie viele "Schritte" wir rotiert haben
        const indexDifference = currentStartIndex - lastStartIndex;

        // Aktualisiere den Rotations-Offset nur wenn sich der Index ändert
        if (indexDifference !== 0) {
            currentRotationOffset += indexDifference * (360 / maxVisibleCards);
            lastStartIndex = currentStartIndex;
        }

        // Smooth interpolation für flüssige Bewegung
        const smoothProgress = scrollProgress * (totalCards - maxVisibleCards);
        const smoothRotation = smoothProgress * (360 / maxVisibleCards);

        // Positioniere die Karten mit kontinuierlicher Rotation
        cards.forEach((card, displayIndex) => {
            if (displayIndex >= maxVisibleCards) return;

            // Berechne die aktuelle Position mit smooth rotation (Startpunkt bei -90°)
            const baseAngle = (360 / maxVisibleCards) * displayIndex;
            const currentAngle = baseAngle - smoothRotation - 180;

            const x = Math.cos(currentAngle * Math.PI / 180) * radius;
            const y = Math.sin(currentAngle * Math.PI / 180) * radius;

            // Setze Transform mit smooth rotation
            card.style.transform = `translate(${x}px, calc(-50% + ${y}px)) rotate(${currentAngle}deg)`;

            // Karten-Content gerade halten und skalieren
            const cardContent = card.querySelector('.scroll-card');
            if (cardContent) {
                // Skalierung basierend auf Position (Karten an den Seiten kleiner)
                const distanceFromTop = Math.abs(y);
                const scale = 1 - (distanceFromTop / (radius * 1.5)) * 0.3;
                const finalScale = Math.max(scale, 0.7);

                cardContent.style.transform = `rotate(${-currentAngle}deg) scale(${finalScale})`;

                // Z-Index für korrekte Überlagerung
                const zIndex = Math.round(100 - Math.abs(x));
                card.style.zIndex = zIndex;
            }
        });

        // Benachrichtige .NET bei Index-Änderungen für Daten-Updates
        if (dotNetRef) {
            dotNetRef.invokeMethodAsync('OnScroll', scrollProgress, currentStartIndex);
        }
    }

    // Event Listeners
    scrollArea.addEventListener('scroll', updateCardsPosition);

    // Mausrad-Unterstützung mit langsamerer Geschwindigkeit für smoothere Rotation
    document.addEventListener('wheel', (e) => {
        if (container.contains(e.target)) {
            e.preventDefault();
            scrollArea.scrollTop += e.deltaY * 1; // Langsamere Geschwindigkeit
        }
    }, { passive: false });

    // Touch-Unterstützung
    let touchStartY = 0;
    container.addEventListener('touchstart', (e) => {
        touchStartY = e.touches[0].clientY;
    }, { passive: true });

    container.addEventListener('touchmove', (e) => {
        e.preventDefault();
        const touchY = e.touches[0].clientY;
        const deltaY = touchStartY - touchY;
        scrollArea.scrollTop += deltaY * 1;
        touchStartY = touchY;
    }, { passive: false });

    // Keyboard-Unterstützung
    document.addEventListener('keydown', (e) => {
        if (container.contains(document.activeElement) || document.activeElement === document.body) {
            switch (e.key) {
                case 'ArrowUp':
                    e.preventDefault();
                    scrollArea.scrollTop -= 30;
                    break;
                case 'ArrowDown':
                    e.preventDefault();
                    scrollArea.scrollTop += 30;
                    break;
                case 'PageUp':
                    e.preventDefault();
                    scrollArea.scrollTop -= 100;
                    break;
                case 'PageDown':
                    e.preventDefault();
                    scrollArea.scrollTop += 100;
                    break;
            }
        }
    });

    // Initiale Position setzen
    updateCardsPosition();
};