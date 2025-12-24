// ApiClient para consumir la API existente (parte: ParteFrontProyectoDSW1)
// Ajusta API_BASE en appsettings o en Layout (window.AppConfig.apiBaseUrl)

const API_BASE = (window.AppConfig && window.AppConfig.apiBaseUrl) ? window.AppConfig.apiBaseUrl.replace(/\/$/, '') + '/' : '/';

const ApiClient = {
    // Session helpers
    setSession(data) {
        // data expected: { token?, usuario? } OR as your API returns
        if (data.token) localStorage.setItem('token', data.token);
        if (data.usuario) localStorage.setItem('user', JSON.stringify(data.usuario));
    },
    clearSession() {
        localStorage.removeItem('token');
        localStorage.removeItem('user');
    },
    getToken() {
        return localStorage.getItem('token');
    },
    getUser() {
        const s = localStorage.getItem('user');
        return s ? JSON.parse(s) : null;
    },

    // Generic fetch with auth header
    async _fetch(endpoint, options = {}) {
        const token = this.getToken();
        const headers = options.headers || {};
        if (!(options.body instanceof FormData)) {
            headers['Content-Type'] = headers['Content-Type'] || 'application/json';
        }
        if (token) headers['Authorization'] = `Bearer ${token}`;

        const resp = await fetch(API_BASE + endpoint, { ...options, headers });
        if (resp.status === 401) {
            this.clearSession();
            window.location.href = '/Account/Login';
            return null;
        }
        if (!resp.ok) {
            const txt = await resp.text();
            throw new Error(txt || resp.statusText);
        }
        // no content
        if (resp.status === 204) return null;
        const ct = resp.headers.get('content-type') || '';
        if (ct.includes('application/json')) return await resp.json();
        return await resp.text();
    },

    // Auth
    async login(email, password) {
        const resp = await this._fetch('api/usuarios/login', {
            method: 'POST',
            body: JSON.stringify({ email, password })
        });
        // repo returns { mensaje, usuario } — adapt if your API returns token
        // If API doesn't return token, consider API change or mocking token locally.
        // We'll try to handle both cases:
        if (!resp) throw new Error('No response');
        // If API returned usuario inside { usuario: {...} } -> store user
        if (resp.usuario) {
            // If token present at top-level, store it
            if (resp.token) localStorage.setItem('token', resp.token);
            localStorage.setItem('user', JSON.stringify(resp.usuario));
            return resp;
        }
        // else if API returned token directly
        if (resp.token || resp.idUsuario) {
            localStorage.setItem('token', resp.token || '');
            localStorage.setItem('user', JSON.stringify({
                idUsuario: resp.idUsuario,
                nombre: resp.nombre,
                email: resp.email,
                rol: resp.rol
            }));
            return resp;
        }
        throw new Error('Formato de respuesta de login inesperado');
    },

    logout() {
        this.clearSession();
        window.location.href = '/';
    },

    // Domain API calls (map to your API controllers)
    getCategorias() { return this._fetch('api/categorias'); },
    getProductos() { return this._fetch('api/productos'); },
    getProductosPorCategoria(idCategoria) { return this._fetch(`api/productos/por-categoria/${idCategoria}`); },

    // Carrito (endpoints depend on API; adjust paths if needed)
    getCarrito() { return this._fetch('api/carrito'); },
    addToCart(productId, quantity) {
        return this._fetch('api/carrito', {
            method: 'POST',
            body: JSON.stringify({ idProducto: productId, cantidad: quantity })
        });
    },
    updateCartItem(idItem, quantity) {
        return this._fetch(`api/carrito/${idItem}`, {
            method: 'PUT',
            body: JSON.stringify({ cantidad: quantity })
        });
    },
    removeCartItem(idItem) {
        return this._fetch(`api/carrito/${idItem}`, { method: 'DELETE' });
    },

    // Ordenes
    createOrder() { return this._fetch('api/orden', { method: 'POST' }); },
    getOrders() { return this._fetch('api/orden'); }
};

window.ApiClient = ApiClient;