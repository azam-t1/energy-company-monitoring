// src/utils/api.js
// Utility for API base URL

// Use runtime config if available, then env variable, then fallback to origin
const apiUrl = 
  (window.REACT_APP_CONFIG && window.REACT_APP_CONFIG.API_URL) ||
  process.env.REACT_APP_API_URL || 
  window.location.origin;

export default apiUrl;
