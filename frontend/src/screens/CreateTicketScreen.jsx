import React, { useState } from 'react'
import { useNavigate } from 'react-router-dom'
import api from '../api/api'

export default function CreateTicketScreen() {
  const [title, setTitle] = useState('')
  const [description, setDescription] = useState('')
  const [severity, setSeverity] = useState('Low')
  const [status, setStatus] = useState('Open')
  const [files, setFiles] = useState([])
  const [isSubmitting, setIsSubmitting] = useState(false)
  const navigate = useNavigate()

  const handleFileChange = (e) => {
    const selectedFiles = Array.from(e.target.files)
    setFiles(selectedFiles)
  }

  const handleSubmit = async (e) => {
    e.preventDefault()
    setIsSubmitting(true)

    try {
      const formData = new FormData()
      formData.append('title', title)
      formData.append('description', description)
      formData.append('severity', severity)
      formData.append('status', status)
      files.forEach(file => {
        formData.append("attachment", file);
      });

      const response = await api.post('/Incident', formData);

      if (response.status >= 200 && response.status < 300) {
        setTitle('')
        setDescription('')
        setSeverity('Low')
        setStatus('Open')
        setFiles([])
        alert('Incident created successfully!')
        navigate('/grid')
      } else {
        throw new Error('Failed to create an Incident')
      }
    } catch (error) {
      console.error('Error creating incident:', error)
      alert('Error creating incident. Please try again.')
    } finally {
      setIsSubmitting(false)
    }
  }

  return (
    <div className="create-ticket-container">
      <h2>Create Incident</h2>
      <form onSubmit={handleSubmit} className="ticket-form">
        <div className="form-group">
          <label htmlFor="title">Title:</label>
          <input
            type="text"
            id="title"
            value={title}
            onChange={(e) => setTitle(e.target.value)}
            required
            placeholder="Enter ticket title"
          />
        </div>

        <div className="form-group full">
          <label htmlFor="description">Description:</label>
          <textarea
            id="description"
            value={description}
            onChange={(e) => setDescription(e.target.value)}
            required
            placeholder="Enter ticket description"
            rows="5"
          />
        </div>

        <div className="form-group">
          <label htmlFor="severity">Severity:</label>
          <select
            id="severity"
            value={severity}
            onChange={(e) => setSeverity(e.target.value)}
            required
          >
            <option value="Low">Low</option>
            <option value="Medium">Medium</option>
            <option value="High">High</option>
          </select>
        </div>

        <div className="form-group">
          <label htmlFor="status">Status:</label>
          <select
            id="status"
            value={status}
            onChange={(e) => setStatus(e.target.value)}
          >
            <option value="Open">Open</option>
            <option value="In Progress">In Progress</option>
            <option value="Resolved">Resolved</option>
            <option value="Closed">Closed</option>
          </select>
        </div>

        <div className="form-group full">
          <label htmlFor="files">Attach Files:</label>
          <input
            type="file"
            id="files"
            multiple
            onChange={handleFileChange}
            accept="image/*,.pdf,.doc,.docx,.txt"
          />
          {files.length > 0 && (
            <div className="file-list">
              <h4>Selected Files:</h4>
              <ul>
                {files.map((file, index) => (
                  <li key={index}>{file.name} ({(file.size / 1024).toFixed(2)} KB)</li>
                ))}
              </ul>
            </div>
          )}
        </div>

        <div className="form-actions full">
          <button type="submit" disabled={isSubmitting} className="submit-btn">
            {isSubmitting ? 'Creating Ticket...' : 'Create Ticket'}
          </button>
        </div>
      </form>
    </div>
  )
}