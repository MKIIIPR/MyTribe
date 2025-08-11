// Token Storage Utilities für Blazor WebAssembly
window.blazorCulture = {
    // Token speichern (verwendet verschlüsselten localStorage)
    set: (key, value) => {
        try {
            // In Produktion sollten Sie eine echte Verschlüsselung verwenden
            const encrypted = btoa(JSON.stringify({ value, timestamp: Date.now() }));
            localStorage.setItem(`jwt_${key}`, encrypted);
        } catch (error) {
            console.error('Token storage error:', error);
        }
    },

    // Token abrufen
    get: (key) => {
        try {
            const encrypted = localStorage.getItem(`jwt_${key}`);
            if (!encrypted) return null;

            const decrypted = JSON.parse(atob(encrypted));

            // Token Expiration Check (Optional)
            if (key === 'accessToken' && decrypted.timestamp) {
                const now = Date.now();
                const tokenAge = now - decrypted.timestamp;
                const maxAge = 60 * 60 * 1000; // 1 Stunde in Millisekunden

                if (tokenAge > maxAge) {
                    localStorage.removeItem(`jwt_${key}`);
                    return null;
                }
            }

            return decrypted.value;
        } catch (error) {
            console.error('Token retrieval error:', error);
            localStorage.removeItem(`jwt_${key}`);
            return null;
        }
    },

    // Token löschen
    remove: (key) => {
        try {
            localStorage.removeItem(`jwt_${key}`);
        } catch (error) {
            console.error('Token removal error:', error);
        }
    },

    // Alle Tokens löschen
    clearAll: () => {
        try {
            const keysToRemove = [];
            for (let i = 0; i < localStorage.length; i++) {
                const key = localStorage.key(i);
                if (key && key.startsWith('jwt_')) {
                    keysToRemove.push(key);
                }
            }
            keysToRemove.forEach(key => localStorage.removeItem(key));
        } catch (error) {
            console.error('Clear all tokens error:', error);
        }
    },

    // Token gültigkeitsprüfung
    isTokenValid: () => {
        try {
            const token = window.blazorCulture.get('accessToken');
            if (!token) return false;

            // JWT Token parsen (ohne Validierung - nur Expiration Check)
            const payload = JSON.parse(atob(token.split('.')[1]));
            const expiration = payload.exp * 1000; // Convert to milliseconds
            const now = Date.now();

            return expiration > now;
        } catch (error) {
            console.error('Token validation error:', error);
            return false;
        }
    },

    // HTTP Authorization Header generieren
    getAuthHeader: () => {
        const token = window.blazorCulture.get('accessToken');
        return token ? `Bearer ${token}` : null;
    }
};

// Automatische Token-Bereinigung bei Browser-Schließung (optional)
window.addEventListener('beforeunload', () => {
    // Nur Refresh Token bei Browser-Schließung löschen, Access Token behalten für Remember Me
    const rememberMe = localStorage.getItem('jwt_rememberMe');
    if (!rememberMe || rememberMe === 'false') {
        window.blazorCulture.clearAll();
    }
});