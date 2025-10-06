output "db_endpoint" {
  description = "RDS instance endpoint"
  value       = aws_db_instance.default.endpoint
}

output "db_port" {
  description = "RDS instance port"
  value       = aws_db_instance.default.port
}

output "db_name" {
  description = "Database name"
  value       = aws_db_instance.default.db_name
}
