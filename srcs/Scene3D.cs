namespace Scop
{
    partial class Scop
    {
		// ======= SHADERS 3D =======
        private static readonly string VertexShaderSource3D = @"
        #version 330 core
        layout (location = 0) in vec3 aPosition;
        layout (location = 1) in vec3 aNormal;
        layout (location = 2) in vec2 aTextureCoord;

        uniform int uHasTexture;

        uniform float uTime;
        uniform int uCamMode;

        uniform mat4 uModel;
        uniform mat4 uView;
        uniform mat4 uProjection;
        uniform vec3 uKd;

        out vec2 frag_texCoords;
        out vec3 frag_normal;
        out vec3 frag_pos;
        out vec3 frag_color;

        void main()
        {
            frag_normal = mat3(transpose(inverse(uModel))) * aNormal;
            frag_pos = vec3(uModel * vec4(aPosition, 1.0));
            gl_Position = uProjection * uView * uModel * vec4(aPosition, 1.0);
            frag_texCoords = aTextureCoord;
            switch (uCamMode)
            {
                case(0):
                {
                    frag_color = vec3(uKd);
                    break;
                }
                case(1):
                {
                    frag_color = vec3(sin(uTime), cos(uTime), 0.5);
                    break;
                }
                case(2):
                {
                    frag_color = vec3(0.0, 0.0, 0.0);
                    break;
                }
            }
        }";

        private static readonly string FragmentShaderSource3D = @"
        #version 330 core
        uniform sampler2D uTexture;

        uniform int uHasTexture;
        uniform int uCamMode;

        uniform vec3 uLightPos;
        uniform vec3 uViewPos;
        uniform vec3 uKa;
        uniform vec3 uKs;
        uniform float uNs;
        uniform float uLightIntensity;

        in vec2 frag_texCoords;
        in vec3 frag_color;
        in vec3 frag_normal;
        in vec3 frag_pos;
        out vec4 out_color;

        void main()
        {
            vec3 baseColor = frag_color;
            if (uCamMode == 0 && uHasTexture == 1)
            {
                baseColor = texture(uTexture, frag_texCoords).rgb;
            }

            vec3 norm = normalize(frag_normal);
            vec3 lightDir = normalize(uLightPos - frag_pos);
            vec3 viewDir = normalize(uViewPos - frag_pos);
            vec3 reflectDir = reflect(-lightDir, norm);

            vec3 ambient = uKa * baseColor;

            float diff = max(dot(norm, lightDir), 0.0);
            vec3 diffuse = diff * baseColor * uLightIntensity;

            float spec = pow(max(dot(viewDir, reflectDir), 0.0), uNs);
            vec3 specular = uKs * spec * uLightIntensity;

            out_color = vec4(ambient + diffuse + specular, 1.0);
        }";

        // ======= GÉOMÉTRIE 3D (cube simple) =======
        private static readonly float[] Vertices3D =
        {
            -0.5f, -0.5f,  0.5f,  0f, 0f, 1f,  0.0f, 0.0f,
            0.5f, -0.5f,  0.5f,  0f, 0f, 1f,  1.0f, 0.0f,
            0.5f,  0.5f,  0.5f,  0f, 0f, 1f,  1.0f, 1.0f,
            -0.5f,  0.5f,  0.5f,  0f, 0f, 1f,  0.0f, 1.0f,

            -0.5f, -0.5f, -0.5f,  0f, 0f,-1f,  1.0f, 0.0f,
            0.5f, -0.5f, -0.5f,  0f, 0f,-1f,  0.0f, 0.0f,
            0.5f,  0.5f, -0.5f,  0f, 0f,-1f,  0.0f, 1.0f,
            -0.5f,  0.5f, -0.5f,  0f, 0f,-1f,  1.0f, 1.0f,

            -0.5f, -0.5f, -0.5f, -1f, 0f, 0f,  0.0f, 0.0f,
            -0.5f, -0.5f,  0.5f, -1f, 0f, 0f,  1.0f, 0.0f,
            -0.5f,  0.5f,  0.5f, -1f, 0f, 0f,  1.0f, 1.0f,
            -0.5f,  0.5f, -0.5f, -1f, 0f, 0f,  0.0f, 1.0f,

            0.5f, -0.5f,  0.5f,  1f, 0f, 0f,  0.0f, 0.0f,
            0.5f, -0.5f, -0.5f,  1f, 0f, 0f,  1.0f, 0.0f,
            0.5f,  0.5f, -0.5f,  1f, 0f, 0f,  1.0f, 1.0f,
            0.5f,  0.5f,  0.5f,  1f, 0f, 0f,  0.0f, 1.0f,

            -0.5f,  0.5f,  0.5f,  0f, 1f, 0f,  0.0f, 0.0f,
            0.5f,  0.5f,  0.5f,  0f, 1f, 0f,  1.0f, 0.0f,
            0.5f,  0.5f, -0.5f,  0f, 1f, 0f,  1.0f, 1.0f,
            -0.5f,  0.5f, -0.5f,  0f, 1f, 0f,  0.0f, 1.0f,

            -0.5f, -0.5f, -0.5f,  0f,-1f, 0f,  0.0f, 0.0f,
            0.5f, -0.5f, -0.5f,  0f,-1f, 0f,  1.0f, 0.0f,
            0.5f, -0.5f,  0.5f,  0f,-1f, 0f,  1.0f, 1.0f,
            -0.5f, -0.5f,  0.5f,  0f,-1f, 0f,  0.0f, 1.0f,
        };

        private static readonly uint[] Indices3D =
        {
            0,  1,  2,   0,  2,  3,  // front
            4,  5,  6,   4,  6,  7,  // back
            8,  9, 10,   8, 10, 11,  // left
            12, 13, 14,  12, 14, 15,  // right
            16, 17, 18,  16, 18, 19,  // top
            20, 21, 22,  20, 22, 23,  // bottom
        };


        // pour la rotation 3D
        private static float _angle = 0f;

        private static float[] _currentVertices;
        private static uint[]  _currentIndices;
	}
}
