import React from 'react'
import { BrowserRouter as Router, Routes, Route, Link, useLocation } from 'react-router-dom'
import { routes } from './routes.jsx'

function App() {
  return (
    <Router>
      <AppContent />
    </Router>
  )
}

function AppContent() {
  const location = useLocation()
  return (
    <div style={{ padding: 20 }}>
      <h1>Incident Management System</h1>
      <nav>
        {routes.map((route) => {
          if (route.path === location.pathname) return null
          return (
            <Link
              key={route.path}
              to={route.path}
              className={`nav-link ${['Create Incident','Grid'].includes(route.title) ? 'primary' : ''}`}
            >
              {route.title}
            </Link>
          )
        })}
      </nav>

      <Routes>
        {routes.map((route) => (
          <Route key={route.path} path={route.path} element={route.element} />
        ))}
      </Routes>
    </div>
  )
}

export default App
