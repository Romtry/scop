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
        uniform mat4 uModel;
        uniform mat4 uView;
        uniform mat4 uProjection;
        out vec2 frag_texCoords;
        void main()
        {
            gl_Position = uProjection * uView * uModel * vec4(aPosition, 1.0);
            frag_texCoords = aTextureCoord;
        }";

        private static readonly string FragmentShaderSource3D = @"
        #version 330 core
        uniform sampler2D uTexture;
        in vec2 frag_texCoords;
        out vec4 out_color;
        void main()
        {
            out_color = texture(uTexture, frag_texCoords);
            out_color = vec4(1.0, 0.0, 0.0, 1.0);
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
