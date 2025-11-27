# RDS Subnet Group
resource "aws_db_subnet_group" "default" {
  name       = "${var.db_name}-subnet-group"
  subnet_ids = var.private_subnet_ids

  tags = {
    Name        = "${var.db_name}-subnet-group"
    Environment = "development"
    Terraform   = "true"
  }
}

resource "aws_db_instance" "default" {
    allocated_storage = 10
    instance_class = "db.t3.micro"
    engine = "postgres"
    db_name = var.db_name
    username = var.db_username
    password = var.db_password

    vpc_security_group_ids = [var.rds_security_group_id]
    db_subnet_group_name = aws_db_subnet_group.default.name
    multi_az = false
    publicly_accessible = false
    skip_final_snapshot = true

    tags = {
        Name = "${var.db_name}-rds-instance"
        Environment = "development" # TODO: make it dynamic
        Terraform = "true"
    }
}
