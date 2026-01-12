import axios from 'axios'

const api = axios.create({
  baseURL: 'https://incident-management-api-dev.azurewebsites.net/api/v1/',
   //baseURL: 'https://localhost:44331/api/v1/',
  //headers: { 'Content-Type': 'application/json' }
});

// Request interceptor
api.interceptors.request.use(
  (config) => {
    // Add auth token if available
    const token = localStorage.getItem('token')
    if (token) {
      config.headers.Authorization = `Bearer ${token}`
    }
    console.log('Making request to:', config.url)
    return config
  },
  (error) => {
    console.error('Request error:', error)
    return Promise.reject(error)
  }
)

// Response interceptor
api.interceptors.response.use(
  (response) => {
    console.log('Response received:', response.status)
    return response
  },
  (error) => {
    console.error('Response error:', error.response?.status, error.message)
    // Handle common errors
    if (error.response?.status === 401) {
      // Handle unauthorized
      console.log('Unauthorized - redirecting to login')
      // You could redirect to login page here
    }
    return Promise.reject(error)
  }
)

export default api
