using OpenTK.Mathematics;

namespace ModelViewer.Core {
    public enum CameraMovement {
        FORWARD,
        BACKWARD,
        LEFT,
        RIGHT,
        UP,
        DOWN
    };

    public class Camera {
        public Vector3 Position;
        public Vector3 Front = Vector3.UnitZ;
        public Vector3 WorldUp = Vector3.UnitY;
        public Vector3 Right;
        public Vector3 Up;

        public float Yaw = -MathHelper.PiOver2;
        public float Pitch = 0.0f;
        public float MovementSpeed = 4.5f;
        public float MouseSensitivity = 0.005f;
        public float FOV = MathHelper.PiOver4;
        public float ZoomSensitivity = 4.0f;

        public float AspectRatio; 

        public Camera(Vector3 position, float aspectRatio) {
            Position = position;
            AspectRatio = aspectRatio;
            updateCameraVectors();
        }

        public Camera(Vector3 position) : this(position, 16.0f / 9.0f) {}
        public Camera(float aspectRatio) : this(new Vector3(0.0f, 0.0f, 0.0f), aspectRatio) {}

        public float GetPitch() {
            return MathHelper.RadiansToDegrees(Pitch);
        }

        public void SetPitch(float value) {
            var angle = MathHelper.Clamp(value, -89.0f, 89.0f);
            Pitch = MathHelper.DegreesToRadians(angle);
            updateCameraVectors();
        }

        public float GetYaw() {
            return MathHelper.RadiansToDegrees(Yaw);
        }

        public void SetYaw(float value) {
            Yaw = MathHelper.DegreesToRadians(value);
            updateCameraVectors();
        }

        public float GetFov() {
            return MathHelper.RadiansToDegrees(FOV);
        }

        public void SetFov(float value) {
            var angle = MathHelper.Clamp(value, 1f, 90f);
            FOV = MathHelper.DegreesToRadians(angle);
        }

        public Matrix4 GetViewMatrix() {
            return Matrix4.LookAt(Position, Position + Front, Up);
        }

        public Matrix4 GetProjectionMatrix() {
            return Matrix4.CreatePerspectiveFieldOfView(FOV, AspectRatio, 0.01f, 100f);
        }

        public void Move(CameraMovement direction, float deltaTime) {
            float velocity = MovementSpeed * deltaTime;

            switch(direction) {
                case CameraMovement.FORWARD: {
                    Position += velocity * Front;
                    break;
                }
                case CameraMovement.BACKWARD: {
                    Position -= velocity * Front;
                    break;
                }
                case CameraMovement.LEFT: {
                    Position -= velocity * Right;
                    break;
                }
                case CameraMovement.RIGHT: {
                    Position += velocity * Right;
                    break;
                }
                case CameraMovement.UP: {
                    Position += velocity * WorldUp;
                    break;
                }
                case CameraMovement.DOWN: {
                    Position -= velocity * WorldUp;
                    break;
                }
                default: break;
            }
        }

        public void Rotate(float xOffset, float yOffset, bool constrainPitch = true) {
            xOffset *= MouseSensitivity;
            yOffset *= MouseSensitivity;

            Yaw += xOffset;
            Pitch -= yOffset;

            if (constrainPitch) {
                float maxPitch = MathHelper.DegreesToRadians(89.5f);
                if(Pitch >  maxPitch) Pitch =  maxPitch;
                if(Pitch < -maxPitch) Pitch = -maxPitch;
            }

            updateCameraVectors();
        }

        public void Zoom(float yOffset) {
            FOV -= MathHelper.DegreesToRadians(yOffset * ZoomSensitivity);
            float minFov = MathHelper.DegreesToRadians(1.0f);
            if(FOV <  minFov) FOV = minFov;
            if(FOV > MathHelper.PiOver2) FOV = MathHelper.PiOver2;
        }

        private void updateCameraVectors() {
            Vector3 front = new Vector3();

            front.X =(float)Math.Cos(Yaw)*(float)Math.Cos(Pitch);
            front.Y = (float)Math.Sin(Pitch);
            front.Z = (float)Math.Sin(Yaw)*(float)Math.Cos(Pitch);
            Front = Vector3.Normalize(front);

            Right = Vector3.Normalize(Vector3.Cross(Front, WorldUp));
            Up = Vector3.Normalize(Vector3.Cross(Right, Front));
        }
    }
}