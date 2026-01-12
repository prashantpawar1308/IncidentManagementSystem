import React, { useEffect, useState } from 'react'
import api from '../api/api'

export default function GridScreen() {
  const [items, setItems] = useState([])
  const [loading, setLoading] = useState(false)
  const [currentPage, setCurrentPage] = useState(1)
  const [totalPages, setTotalPages] = useState(1)
  const itemsPerPage = 10

  const [modalOpen, setModalOpen] = useState(false)
  const [modalSrc, setModalSrc] = useState('')

  // Inline editing state
  const [editingId, setEditingId] = useState(null)
  const [editStatus, setEditStatus] = useState('')
  const [savingId, setSavingId] = useState(null)

  // Filters (header-level)
  const [filterStatus, setFilterStatus] = useState('All')
  const [filterSeverity, setFilterSeverity] = useState('All')

  const clearFilters = () => {
    setFilterStatus('All')
    setFilterSeverity('All')
    setCurrentPage(1)
  }

  const openModal = (src) => {
    setModalSrc(src)
    setModalOpen(true)
  }

  const closeModal = () => {
    setModalOpen(false)
    setModalSrc('')
  }

  const startEdit = (id, status) => {
    setEditingId(id)
    setEditStatus(status || 'Open')
  }

  const cancelEdit = () => {
    setEditingId(null)
    setEditStatus('')
  }

  const saveEdit = async (id) => {
    try {
      setSavingId(id)
      const payload = { status: editStatus }
      const res = await api.put(`/Incident/${id}`, payload)
      if (res.status >= 200 && res.status < 300) {
        setItems(prev => prev.map(it => it.id === id ? { ...it, status: editStatus } : it))
        setEditingId(null)
        setEditStatus('')
        alert('Status updated successfully')
      } else {
        throw new Error('Failed to update')
      }
    } catch (err) {
      console.error('Update error:', err)
      alert('Failed to update status. Please try again.')
    } finally {
      setSavingId(null)
    }
  }

  useEffect(() => {
    let mounted = true
    setLoading(true)

    // Calculate offset for pagination
    const offset = (currentPage - 1) * itemsPerPage

    api.get(`/Incident`) //our api call
      .then((res) => {
        if (mounted) {
          setItems(res.data)
          // Calculate total pages based on total count from headers
          const totalCount = 10;//parseInt(res.headers['x-total-count']) || 5000 // JSONPlaceholder has ~5000 photos
          setTotalPages(Math.ceil(totalCount / itemsPerPage))
        }
      })
      .catch(() => {})
      .finally(() => { if (mounted) setLoading(false) })

    return () => { mounted = false }
  }, [currentPage])

  const handlePageChange = (page) => {
    if (page >= 1 && page <= totalPages) {
      setCurrentPage(page)
    }
  }

  const renderPagination = () => {
    const pages = []
    const maxVisiblePages = 5
    let startPage = Math.max(1, currentPage - Math.floor(maxVisiblePages / 2))
    let endPage = Math.min(totalPages, startPage + maxVisiblePages - 1)

    // Adjust start page if we're near the end
    if (endPage - startPage + 1 < maxVisiblePages) {
      startPage = Math.max(1, endPage - maxVisiblePages + 1)
    }

    // Add first page and ellipsis if needed
    if (startPage > 1) {
      pages.push(
        <button key={1} onClick={() => handlePageChange(1)} className="page-btn">
          1
        </button>
      )
      if (startPage > 2) {
        pages.push(<span key="start-ellipsis" className="pagination-ellipsis">...</span>)
      }
    }

    // Add visible page numbers
    for (let i = startPage; i <= endPage; i++) {
      pages.push(
        <button
          key={i}
          onClick={() => handlePageChange(i)}
          className={`page-btn ${i === currentPage ? 'active' : ''}`}
        >
          {i}
        </button>
      )
    }

    // Add last page and ellipsis if needed
    if (endPage < totalPages) {
      if (endPage < totalPages - 1) {
        pages.push(<span key="end-ellipsis" className="pagination-ellipsis">...</span>)
      }
      pages.push(
        <button key={totalPages} onClick={() => handlePageChange(totalPages)} className="page-btn">
          {totalPages}
        </button>
      )
    }

    return pages
  }

  const filteredItems = items.filter(it => {
    const matchesStatus = filterStatus === 'All' || it.status === filterStatus
    const matchesSeverity = filterSeverity === 'All' || it.severity === filterSeverity
    return matchesStatus && matchesSeverity
  })
  const offset = (currentPage - 1) * itemsPerPage
  const paginatedItems = filteredItems.slice(offset, offset + itemsPerPage)

  return (
    <div>
      <h2>Incidents</h2>
      {loading ? (
        <div>Loading...</div>
      ) : (
        <>
          <table className="photo-table">
            <thead>
              <tr>
                <th>ID</th>
                <th>Title</th>
                <th>Description</th>
                <th>Severity</th>
                <th>Status</th>
                <th>Attachment</th>
                <th>Actions</th>
              </tr>
              <tr className="header-filters">
                <th></th>
                <th></th>
                <th></th>
                <th>
                  <select value={filterSeverity} onChange={(e) => { setFilterSeverity(e.target.value); setCurrentPage(1); }}>
                    <option value="All">All</option>
                    <option value="Low">Low</option>
                    <option value="Medium">Medium</option>
                    <option value="High">High</option>
                  </select>
                </th>
                <th>
                  <select value={filterStatus} onChange={(e) => { setFilterStatus(e.target.value); setCurrentPage(1); }}>
                    <option value="All">All</option>
                    <option value="Open">Open</option>
                    <option value="In Progress">In Progress</option>
                    <option value="Resolved">Resolved</option>
                    <option value="Closed">Closed</option>
                  </select>
                </th>
                <th></th>
                <th>
                  <button className="clear-filters" onClick={clearFilters}>Clear</button>
                </th>
              </tr>  
            </thead>
            <tbody>
              {paginatedItems.map((it) => (
                <tr key={it.id}>
                  <td>{it.id}</td>
                  <td>{it.title}</td>
                  <td>{it.description}</td> 
                  <td>{it.severity}</td>
                  <td>
                    {editingId === it.id ? (
                      <select value={editStatus} onChange={(e) => setEditStatus(e.target.value)}>
                        <option value="Open">Open</option>
                        <option value="In Progress">In Progress</option>
                        <option value="Resolved">Resolved</option>
                        <option value="Closed">Closed</option>
                      </select>
                    ) : (
                      it.status || '—'
                    )}
                  </td>
                  <td>
                    {it.attachmentPath ? (
                      <a href={it.attachmentPath}
                        target="_blank"
                        rel="noopener noreferrer"
                      >
                        Download
                      </a>
                    ) : (
                      '—'
                    )}
                  </td>
                  <td className="actions-cell">
                    {editingId === it.id ? (
                      <>
                        <button className="save-btn" onClick={() => saveEdit(it.id)} disabled={savingId === it.id}>
                          {savingId === it.id ? 'Saving...' : 'Save'}
                        </button>
                        <button className="cancel-btn" onClick={cancelEdit}>Cancel</button>
                      </>
                    ) : (
                      <button className="page-btn" onClick={() => startEdit(it.id, it.status)}>Edit</button>
                    )}
                  </td>
                </tr>
              ))}
            </tbody>
          </table>

          <div className="pagination">
            <button
              onClick={() => handlePageChange(currentPage - 1)}
              disabled={currentPage === 1}
              className="page-btn nav-btn"
            >
              Previous
            </button>

            <div className="page-numbers">
              {renderPagination()}
            </div>

            <button
              onClick={() => handlePageChange(currentPage + 1)}
              disabled={currentPage === totalPages}
              className="page-btn nav-btn"
            >
              Next
            </button>
          </div>

          <div className="pagination-info">
            Page {currentPage} of {totalPages} ({filteredItems.length} items)
          </div>

          {modalOpen && (
            <div className="image-modal" onClick={closeModal}>
              <div className="image-modal-content" onClick={(e) => e.stopPropagation()}>
                <button className="close-btn" onClick={closeModal} aria-label="Close">×</button>
                <img src={modalSrc} alt="Full attachment" />
              </div>
            </div>
          )}
        </>
      )}
    </div>
  )
}
