using System;
using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace Unify.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class diddly : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "faculties",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    name = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_faculties", x => x.id);
                });

            migrationBuilder.CreateTable(
                name: "grade",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    description = table.Column<string>(type: "character varying(400)", maxLength: 400, nullable: false),
                    date_awarded = table.Column<DateTime>(type: "timestamp with time zone", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_grade", x => x.id);
                });

            migrationBuilder.CreateTable(
                name: "homework_bases_attachments",
                columns: table => new
                {
                    homework_base_id = table.Column<Guid>(type: "uuid", nullable: false),
                    attachment_id = table.Column<Guid>(type: "uuid", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_homework_bases_attachments", x => new { x.attachment_id, x.homework_base_id });
                });

            migrationBuilder.CreateTable(
                name: "permissions",
                columns: table => new
                {
                    id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    name = table.Column<string>(type: "text", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_permissions", x => x.id);
                });

            migrationBuilder.CreateTable(
                name: "roles",
                columns: table => new
                {
                    id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    name = table.Column<string>(type: "text", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_roles", x => x.id);
                });

            migrationBuilder.CreateTable(
                name: "fields_of_study",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    name = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: false),
                    description = table.Column<string>(type: "character varying(800)", maxLength: 800, nullable: false),
                    faculty_id = table.Column<Guid>(type: "uuid", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_fields_of_study", x => x.id);
                    table.ForeignKey(
                        name: "fk_fields_of_study_faculties_faculty_id",
                        column: x => x.faculty_id,
                        principalTable: "faculties",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "locations",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    building = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: true),
                    street = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: true),
                    floor = table.Column<short>(type: "smallint", nullable: true),
                    door_number = table.Column<string>(type: "character varying(20)", maxLength: 20, nullable: true),
                    faculty_id = table.Column<Guid>(type: "uuid", nullable: true),
                    online = table.Column<bool>(type: "boolean", nullable: false),
                    meeting_url = table.Column<string>(type: "character varying(400)", maxLength: 400, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_locations", x => x.id);
                    table.ForeignKey(
                        name: "fk_locations_faculties_faculty_id",
                        column: x => x.faculty_id,
                        principalTable: "faculties",
                        principalColumn: "id");
                });

            migrationBuilder.CreateTable(
                name: "marks",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    grade_id = table.Column<Guid>(type: "uuid", nullable: false),
                    submission_id = table.Column<Guid>(type: "uuid", nullable: true),
                    criteria = table.Column<string>(type: "character varying(400)", maxLength: 400, nullable: true),
                    score = table.Column<decimal>(type: "numeric", nullable: false),
                    max_score = table.Column<decimal>(type: "numeric", nullable: false),
                    homework_mark = table.Column<bool>(type: "boolean", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_marks", x => x.id);
                    table.ForeignKey(
                        name: "fk_marks_grade_grade_id",
                        column: x => x.grade_id,
                        principalTable: "grade",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "role_permissions",
                columns: table => new
                {
                    role_id = table.Column<int>(type: "integer", nullable: false),
                    permission_id = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_role_permissions", x => new { x.role_id, x.permission_id });
                    table.ForeignKey(
                        name: "fk_role_permissions_permissions_permission_id",
                        column: x => x.permission_id,
                        principalTable: "permissions",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "fk_role_permissions_roles_role_id",
                        column: x => x.role_id,
                        principalTable: "roles",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "specializations",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    name = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: false),
                    description = table.Column<string>(type: "character varying(800)", maxLength: 800, nullable: false),
                    field_of_study_id = table.Column<Guid>(type: "uuid", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_specializations", x => x.id);
                    table.ForeignKey(
                        name: "fk_specializations_fields_of_study_field_of_study_id",
                        column: x => x.field_of_study_id,
                        principalTable: "fields_of_study",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "student_group",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    name = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: false),
                    specialization_id = table.Column<Guid>(type: "uuid", nullable: false),
                    study_year = table.Column<int>(type: "integer", nullable: false),
                    semester = table.Column<int>(type: "integer", nullable: false),
                    term = table.Column<int>(type: "integer", nullable: false),
                    max_group_size = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_student_group", x => x.id);
                    table.ForeignKey(
                        name: "fk_student_group_specializations_specialization_id",
                        column: x => x.specialization_id,
                        principalTable: "specializations",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "users",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    first_name = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: false),
                    last_name = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: false),
                    email = table.Column<string>(type: "character varying(400)", maxLength: 400, nullable: false),
                    identity_id = table.Column<string>(type: "text", nullable: false),
                    student_group_id = table.Column<Guid>(type: "uuid", nullable: true),
                    specialization_id = table.Column<Guid>(type: "uuid", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_users", x => x.id);
                    table.ForeignKey(
                        name: "fk_users_specializations_specialization_id",
                        column: x => x.specialization_id,
                        principalTable: "specializations",
                        principalColumn: "id");
                    table.ForeignKey(
                        name: "fk_users_student_group_student_group_id",
                        column: x => x.student_group_id,
                        principalTable: "student_group",
                        principalColumn: "id");
                });

            migrationBuilder.CreateTable(
                name: "courses",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    name = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: false),
                    description = table.Column<string>(type: "character varying(400)", maxLength: 400, nullable: false),
                    specialization_id = table.Column<Guid>(type: "uuid", nullable: false),
                    lecturer_id = table.Column<Guid>(type: "uuid", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_courses", x => x.id);
                    table.ForeignKey(
                        name: "fk_courses_specialization_specialization_id",
                        column: x => x.specialization_id,
                        principalTable: "specializations",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "fk_courses_user_lecturer_id",
                        column: x => x.lecturer_id,
                        principalTable: "users",
                        principalColumn: "id");
                });

            migrationBuilder.CreateTable(
                name: "information_messages",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    sender_id = table.Column<Guid>(type: "uuid", nullable: false),
                    title = table.Column<string>(type: "character varying(400)", maxLength: 400, nullable: false),
                    content = table.Column<string>(type: "text", nullable: false),
                    expiration_date = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    created_on = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    severity_level = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_information_messages", x => x.id);
                    table.ForeignKey(
                        name: "fk_information_messages_user_sender_id",
                        column: x => x.sender_id,
                        principalTable: "users",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "messages",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    title = table.Column<string>(type: "character varying(400)", maxLength: 400, nullable: false),
                    sender_id = table.Column<Guid>(type: "uuid", nullable: false),
                    content = table.Column<string>(type: "text", nullable: false),
                    responding_to_message_id = table.Column<Guid>(type: "uuid", nullable: true),
                    forwarded_from_message_id = table.Column<Guid>(type: "uuid", nullable: true),
                    status = table.Column<int>(type: "integer", nullable: false),
                    created_on = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_messages", x => x.id);
                    table.ForeignKey(
                        name: "fk_messages_user_sender_id",
                        column: x => x.sender_id,
                        principalTable: "users",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "role_user",
                columns: table => new
                {
                    roles_id = table.Column<int>(type: "integer", nullable: false),
                    users_id = table.Column<Guid>(type: "uuid", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_role_user", x => new { x.roles_id, x.users_id });
                    table.ForeignKey(
                        name: "fk_role_user_role_roles_id",
                        column: x => x.roles_id,
                        principalTable: "roles",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "fk_role_user_user_users_id",
                        column: x => x.users_id,
                        principalTable: "users",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "class_offerings",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    name = table.Column<string>(type: "character varying(60)", maxLength: 60, nullable: false),
                    course_id = table.Column<Guid>(type: "uuid", nullable: false),
                    start_date = table.Column<DateOnly>(type: "date", nullable: false),
                    end_date = table.Column<DateOnly>(type: "date", nullable: false),
                    lecturer_id = table.Column<Guid>(type: "uuid", nullable: false),
                    student_group_id = table.Column<Guid>(type: "uuid", nullable: false),
                    max_students_count = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_class_offerings", x => x.id);
                    table.ForeignKey(
                        name: "fk_class_offerings_course_course_id",
                        column: x => x.course_id,
                        principalTable: "courses",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "fk_class_offerings_student_group_student_group_id",
                        column: x => x.student_group_id,
                        principalTable: "student_group",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "fk_class_offerings_user_lecturer_id",
                        column: x => x.lecturer_id,
                        principalTable: "users",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "course_resources",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    course_id = table.Column<Guid>(type: "uuid", nullable: false),
                    title = table.Column<string>(type: "character varying(60)", maxLength: 60, nullable: false),
                    description = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_course_resources", x => x.id);
                    table.ForeignKey(
                        name: "fk_course_resources_course_course_id",
                        column: x => x.course_id,
                        principalTable: "courses",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "lectures",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    course_id = table.Column<Guid>(type: "uuid", nullable: false),
                    title = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    class_type = table.Column<int>(type: "integer", nullable: false),
                    scheduled_date = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    duration = table.Column<TimeSpan>(type: "interval", nullable: false),
                    lecturer_id = table.Column<Guid>(type: "uuid", nullable: false),
                    location_id = table.Column<Guid>(type: "uuid", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_lectures", x => x.id);
                    table.ForeignKey(
                        name: "fk_lectures_course_course_id",
                        column: x => x.course_id,
                        principalTable: "courses",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "fk_lectures_location_location_id",
                        column: x => x.location_id,
                        principalTable: "locations",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "fk_lectures_user_lecturer_id",
                        column: x => x.lecturer_id,
                        principalTable: "users",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "information_message_user",
                columns: table => new
                {
                    information_message_id = table.Column<Guid>(type: "uuid", nullable: false),
                    user_id = table.Column<Guid>(type: "uuid", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_information_message_user", x => new { x.information_message_id, x.user_id });
                    table.ForeignKey(
                        name: "fk_information_message_user_information_messages_information_m",
                        column: x => x.information_message_id,
                        principalTable: "information_messages",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "fk_information_message_user_user_user_id",
                        column: x => x.user_id,
                        principalTable: "users",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "messages_users",
                columns: table => new
                {
                    message_id = table.Column<Guid>(type: "uuid", nullable: false),
                    user_id = table.Column<Guid>(type: "uuid", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_messages_users", x => new { x.message_id, x.user_id });
                    table.ForeignKey(
                        name: "fk_messages_users_messages_message_id",
                        column: x => x.message_id,
                        principalTable: "messages",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "fk_messages_users_user_user_id",
                        column: x => x.user_id,
                        principalTable: "users",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "class_enrollments",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    class_offering_id = table.Column<Guid>(type: "uuid", nullable: false),
                    student_id = table.Column<Guid>(type: "uuid", nullable: false),
                    student_group_id = table.Column<Guid>(type: "uuid", nullable: false),
                    enrolled_on = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    grade_id = table.Column<Guid>(type: "uuid", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_class_enrollments", x => x.id);
                    table.ForeignKey(
                        name: "fk_class_enrollments_class_offering_class_offering_id",
                        column: x => x.class_offering_id,
                        principalTable: "class_offerings",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "fk_class_enrollments_grade_grade_id",
                        column: x => x.grade_id,
                        principalTable: "grade",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "fk_class_enrollments_student_group_student_group_id",
                        column: x => x.student_group_id,
                        principalTable: "student_group",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "fk_class_enrollments_user_student_id",
                        column: x => x.student_id,
                        principalTable: "users",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "class_offering_sessions",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    class_offering_id = table.Column<Guid>(type: "uuid", nullable: false),
                    title = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    class_type = table.Column<int>(type: "integer", nullable: false),
                    scheduled_date = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    duration = table.Column<TimeSpan>(type: "interval", nullable: false),
                    lecturer_id = table.Column<Guid>(type: "uuid", nullable: false),
                    location_id = table.Column<Guid>(type: "uuid", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_class_offering_sessions", x => x.id);
                    table.ForeignKey(
                        name: "fk_class_offering_sessions_class_offerings_class_offering_id",
                        column: x => x.class_offering_id,
                        principalTable: "class_offerings",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "fk_class_offering_sessions_location_location_id",
                        column: x => x.location_id,
                        principalTable: "locations",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "fk_class_offering_sessions_user_lecturer_id",
                        column: x => x.lecturer_id,
                        principalTable: "users",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "homework_assignments",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    class_offering_id = table.Column<Guid>(type: "uuid", nullable: false),
                    title = table.Column<string>(type: "character varying(60)", maxLength: 60, nullable: false),
                    description = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: false),
                    due_date = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    locked = table.Column<bool>(type: "boolean", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_homework_assignments", x => x.id);
                    table.ForeignKey(
                        name: "fk_homework_assignments_class_offering_class_offering_id",
                        column: x => x.class_offering_id,
                        principalTable: "class_offerings",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "offering_resources",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    class_offering_id = table.Column<Guid>(type: "uuid", nullable: false),
                    title = table.Column<string>(type: "character varying(60)", maxLength: 60, nullable: false),
                    description = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_offering_resources", x => x.id);
                    table.ForeignKey(
                        name: "fk_offering_resources_class_offering_class_offering_id",
                        column: x => x.class_offering_id,
                        principalTable: "class_offerings",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "homework_submissions",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    homework_assigment_id = table.Column<Guid>(type: "uuid", nullable: false),
                    student_id = table.Column<Guid>(type: "uuid", nullable: false),
                    mark_id = table.Column<Guid>(type: "uuid", nullable: true),
                    feedback = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: true),
                    submitted_on = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    homework_assignment_id = table.Column<Guid>(type: "uuid", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_homework_submissions", x => x.id);
                    table.ForeignKey(
                        name: "fk_homework_submissions_homework_assignments_homework_assigmen",
                        column: x => x.homework_assigment_id,
                        principalTable: "homework_assignments",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "fk_homework_submissions_homework_assignments_homework_assignme",
                        column: x => x.homework_assignment_id,
                        principalTable: "homework_assignments",
                        principalColumn: "id");
                    table.ForeignKey(
                        name: "fk_homework_submissions_mark_mark_id",
                        column: x => x.mark_id,
                        principalTable: "marks",
                        principalColumn: "id");
                    table.ForeignKey(
                        name: "fk_homework_submissions_user_student_id",
                        column: x => x.student_id,
                        principalTable: "users",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "attachments",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    file_name = table.Column<string>(type: "text", nullable: false),
                    data = table.Column<byte[]>(type: "bytea", nullable: false),
                    homework_assignment_id = table.Column<Guid>(type: "uuid", nullable: true),
                    homework_submission_id = table.Column<Guid>(type: "uuid", nullable: true),
                    course_resources_id = table.Column<Guid>(type: "uuid", nullable: true),
                    offering_resources_id = table.Column<Guid>(type: "uuid", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_attachments", x => x.id);
                    table.ForeignKey(
                        name: "fk_attachments_course_resources_course_resources_id",
                        column: x => x.course_resources_id,
                        principalTable: "course_resources",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "fk_attachments_homework_assignment_homework_assignment_id",
                        column: x => x.homework_assignment_id,
                        principalTable: "homework_assignments",
                        principalColumn: "id");
                    table.ForeignKey(
                        name: "fk_attachments_homework_submission_homework_submission_id",
                        column: x => x.homework_submission_id,
                        principalTable: "homework_submissions",
                        principalColumn: "id");
                    table.ForeignKey(
                        name: "fk_attachments_offering_resources_offering_resources_id",
                        column: x => x.offering_resources_id,
                        principalTable: "offering_resources",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "information_message_attachments",
                columns: table => new
                {
                    information_message_id = table.Column<Guid>(type: "uuid", nullable: false),
                    attachment_id = table.Column<Guid>(type: "uuid", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_information_message_attachments", x => new { x.information_message_id, x.attachment_id });
                    table.ForeignKey(
                        name: "fk_information_message_attachments_attachment_attachment_id",
                        column: x => x.attachment_id,
                        principalTable: "attachments",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "fk_information_message_attachments_information_messages_inform",
                        column: x => x.information_message_id,
                        principalTable: "information_messages",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "message_attachments",
                columns: table => new
                {
                    message_id = table.Column<Guid>(type: "uuid", nullable: false),
                    attachment_id = table.Column<Guid>(type: "uuid", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_message_attachments", x => new { x.message_id, x.attachment_id });
                    table.ForeignKey(
                        name: "fk_message_attachments_attachment_attachment_id",
                        column: x => x.attachment_id,
                        principalTable: "attachments",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "fk_message_attachments_messages_message_id",
                        column: x => x.message_id,
                        principalTable: "messages",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.InsertData(
                table: "permissions",
                columns: new[] { "id", "name" },
                values: new object[] { 1, "users:read" });

            migrationBuilder.InsertData(
                table: "roles",
                columns: new[] { "id", "name" },
                values: new object[,]
                {
                    { 1, "Registered" },
                    { 2, "Administrator" },
                    { 3, "Student" },
                    { 4, "Lecturer" }
                });

            migrationBuilder.InsertData(
                table: "role_permissions",
                columns: new[] { "permission_id", "role_id" },
                values: new object[] { 1, 1 });

            migrationBuilder.CreateIndex(
                name: "ix_attachments_course_resources_id",
                table: "attachments",
                column: "course_resources_id");

            migrationBuilder.CreateIndex(
                name: "ix_attachments_homework_assignment_id",
                table: "attachments",
                column: "homework_assignment_id");

            migrationBuilder.CreateIndex(
                name: "ix_attachments_homework_submission_id",
                table: "attachments",
                column: "homework_submission_id");

            migrationBuilder.CreateIndex(
                name: "ix_attachments_offering_resources_id",
                table: "attachments",
                column: "offering_resources_id");

            migrationBuilder.CreateIndex(
                name: "ix_class_enrollments_class_offering_id",
                table: "class_enrollments",
                column: "class_offering_id");

            migrationBuilder.CreateIndex(
                name: "ix_class_enrollments_grade_id",
                table: "class_enrollments",
                column: "grade_id");

            migrationBuilder.CreateIndex(
                name: "ix_class_enrollments_student_group_id",
                table: "class_enrollments",
                column: "student_group_id");

            migrationBuilder.CreateIndex(
                name: "ix_class_enrollments_student_id",
                table: "class_enrollments",
                column: "student_id");

            migrationBuilder.CreateIndex(
                name: "ix_class_offering_sessions_class_offering_id",
                table: "class_offering_sessions",
                column: "class_offering_id");

            migrationBuilder.CreateIndex(
                name: "ix_class_offering_sessions_lecturer_id",
                table: "class_offering_sessions",
                column: "lecturer_id");

            migrationBuilder.CreateIndex(
                name: "ix_class_offering_sessions_location_id",
                table: "class_offering_sessions",
                column: "location_id");

            migrationBuilder.CreateIndex(
                name: "ix_class_offerings_course_id",
                table: "class_offerings",
                column: "course_id");

            migrationBuilder.CreateIndex(
                name: "ix_class_offerings_lecturer_id",
                table: "class_offerings",
                column: "lecturer_id");

            migrationBuilder.CreateIndex(
                name: "ix_class_offerings_student_group_id",
                table: "class_offerings",
                column: "student_group_id");

            migrationBuilder.CreateIndex(
                name: "ix_course_resources_course_id",
                table: "course_resources",
                column: "course_id");

            migrationBuilder.CreateIndex(
                name: "ix_courses_lecturer_id",
                table: "courses",
                column: "lecturer_id");

            migrationBuilder.CreateIndex(
                name: "ix_courses_specialization_id",
                table: "courses",
                column: "specialization_id");

            migrationBuilder.CreateIndex(
                name: "ix_fields_of_study_faculty_id",
                table: "fields_of_study",
                column: "faculty_id");

            migrationBuilder.CreateIndex(
                name: "ix_homework_assignments_class_offering_id",
                table: "homework_assignments",
                column: "class_offering_id");

            migrationBuilder.CreateIndex(
                name: "ix_homework_submissions_homework_assigment_id",
                table: "homework_submissions",
                column: "homework_assigment_id");

            migrationBuilder.CreateIndex(
                name: "ix_homework_submissions_homework_assignment_id",
                table: "homework_submissions",
                column: "homework_assignment_id");

            migrationBuilder.CreateIndex(
                name: "ix_homework_submissions_mark_id",
                table: "homework_submissions",
                column: "mark_id");

            migrationBuilder.CreateIndex(
                name: "ix_homework_submissions_student_id",
                table: "homework_submissions",
                column: "student_id");

            migrationBuilder.CreateIndex(
                name: "ix_information_message_attachments_attachment_id",
                table: "information_message_attachments",
                column: "attachment_id");

            migrationBuilder.CreateIndex(
                name: "ix_information_message_user_user_id",
                table: "information_message_user",
                column: "user_id");

            migrationBuilder.CreateIndex(
                name: "ix_information_messages_sender_id",
                table: "information_messages",
                column: "sender_id");

            migrationBuilder.CreateIndex(
                name: "ix_lectures_course_id",
                table: "lectures",
                column: "course_id");

            migrationBuilder.CreateIndex(
                name: "ix_lectures_lecturer_id",
                table: "lectures",
                column: "lecturer_id");

            migrationBuilder.CreateIndex(
                name: "ix_lectures_location_id",
                table: "lectures",
                column: "location_id");

            migrationBuilder.CreateIndex(
                name: "ix_locations_faculty_id",
                table: "locations",
                column: "faculty_id");

            migrationBuilder.CreateIndex(
                name: "ix_marks_grade_id",
                table: "marks",
                column: "grade_id");

            migrationBuilder.CreateIndex(
                name: "ix_message_attachments_attachment_id",
                table: "message_attachments",
                column: "attachment_id");

            migrationBuilder.CreateIndex(
                name: "ix_messages_sender_id",
                table: "messages",
                column: "sender_id");

            migrationBuilder.CreateIndex(
                name: "ix_messages_users_user_id",
                table: "messages_users",
                column: "user_id");

            migrationBuilder.CreateIndex(
                name: "ix_offering_resources_class_offering_id",
                table: "offering_resources",
                column: "class_offering_id");

            migrationBuilder.CreateIndex(
                name: "ix_role_permissions_permission_id",
                table: "role_permissions",
                column: "permission_id");

            migrationBuilder.CreateIndex(
                name: "ix_role_user_users_id",
                table: "role_user",
                column: "users_id");

            migrationBuilder.CreateIndex(
                name: "ix_specializations_field_of_study_id",
                table: "specializations",
                column: "field_of_study_id");

            migrationBuilder.CreateIndex(
                name: "ix_student_group_specialization_id",
                table: "student_group",
                column: "specialization_id");

            migrationBuilder.CreateIndex(
                name: "ix_users_email",
                table: "users",
                column: "email",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "ix_users_identity_id",
                table: "users",
                column: "identity_id",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "ix_users_specialization_id",
                table: "users",
                column: "specialization_id");

            migrationBuilder.CreateIndex(
                name: "ix_users_student_group_id",
                table: "users",
                column: "student_group_id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "class_enrollments");

            migrationBuilder.DropTable(
                name: "class_offering_sessions");

            migrationBuilder.DropTable(
                name: "homework_bases_attachments");

            migrationBuilder.DropTable(
                name: "information_message_attachments");

            migrationBuilder.DropTable(
                name: "information_message_user");

            migrationBuilder.DropTable(
                name: "lectures");

            migrationBuilder.DropTable(
                name: "message_attachments");

            migrationBuilder.DropTable(
                name: "messages_users");

            migrationBuilder.DropTable(
                name: "role_permissions");

            migrationBuilder.DropTable(
                name: "role_user");

            migrationBuilder.DropTable(
                name: "information_messages");

            migrationBuilder.DropTable(
                name: "locations");

            migrationBuilder.DropTable(
                name: "attachments");

            migrationBuilder.DropTable(
                name: "messages");

            migrationBuilder.DropTable(
                name: "permissions");

            migrationBuilder.DropTable(
                name: "roles");

            migrationBuilder.DropTable(
                name: "course_resources");

            migrationBuilder.DropTable(
                name: "homework_submissions");

            migrationBuilder.DropTable(
                name: "offering_resources");

            migrationBuilder.DropTable(
                name: "homework_assignments");

            migrationBuilder.DropTable(
                name: "marks");

            migrationBuilder.DropTable(
                name: "class_offerings");

            migrationBuilder.DropTable(
                name: "grade");

            migrationBuilder.DropTable(
                name: "courses");

            migrationBuilder.DropTable(
                name: "users");

            migrationBuilder.DropTable(
                name: "student_group");

            migrationBuilder.DropTable(
                name: "specializations");

            migrationBuilder.DropTable(
                name: "fields_of_study");

            migrationBuilder.DropTable(
                name: "faculties");
        }
    }
}
